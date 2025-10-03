#!/usr/bin/env bash
set -Eeuo pipefail

if [[ ${EUID:-$(id -u)} -ne 0 ]]; then
  echo "[FATAL] این اسکریپت باید با کاربر ریشه (root) اجرا شود." >&2
  exit 1
fi

umask 027

log() {
  local level="$1"; shift
  printf '[%s] [%s] %s\n' "$(date --iso-8601=seconds)" "$level" "$*"
}

trap 'log FATAL "خطا در اجرای اسکریپت در خط ${LINENO}."' ERR

update_system_packages() {
  log INFO "به‌روزرسانی مخازن و بسته‌های سیستم"
  apt-get update
  DEBIAN_FRONTEND=noninteractive apt-get -y upgrade
  apt-get -y autoremove
}

install_base_packages() {
  log INFO "نصب بسته‌های پایه مورد نیاز"
  local packages=(curl git ufw nginx certbot python3-certbot-nginx fail2ban ca-certificates gnupg lsb-release)
  DEBIAN_FRONTEND=noninteractive apt-get install -y "${packages[@]}"
}

configure_firewall() {
  log INFO "پیکربندی فایروال UFW"
  ufw allow OpenSSH
  ufw allow http
  ufw allow https
  ufw --force enable
}

require_env() {
  local var_name="$1";
  if [[ -z "${!var_name:-}" ]]; then
    log FATAL "متغیر محیطی ${var_name} تنظیم نشده است.";
    exit 1;
  fi
}

require_dns_record() {
  local domain="$1"
  if [[ -z "$domain" ]]; then
    log FATAL "دامنه برای بررسی DNS ارائه نشده است."
    exit 1
  fi
  log INFO "تأیید رکورد DNS برای دامنه ${domain}"
  local lookup_output=""
  lookup_output=$(getent ahosts "$domain" || true)
  if [[ -z "$lookup_output" ]]; then
    log FATAL "رکورد DNS برای دامنه ${domain} یافت نشد. لطفاً قبل از فعال‌سازی TLS رکورد A یا AAAA را ایجاد کنید یا متغیر EAZY_ENABLE_TLS=false را تنظیم کنید."
    exit 1
  fi
  log INFO "رکورد DNS یافت شد: ${lookup_output%% *}"
}

# ----- متغیرهای پیکربندی -----
APP_USER="${EAZY_APP_USER:-eazymenu}"
APP_GROUP="$APP_USER"
APP_ROOT="${EAZY_APP_ROOT:-/opt/eazymenu}"
SRC_DIR="$APP_ROOT/src"
COMPOSE_DIR="$SRC_DIR/deploy/docker"
COMPOSE_FILE="$COMPOSE_DIR/docker-compose.yml"
COMPOSE_ENV_FILE="$COMPOSE_DIR/.env"
ENV_DIR="/etc/eazymenu"
LOG_DIR="/var/log/eazymenu"
SQL_CONTAINER_NAME="${EAZY_SQL_CONTAINER_NAME:-eazymenu-sql}"
SQL_VOLUME_NAME="${EAZY_SQL_VOLUME_NAME:-eazymenu-sql-data}"
SQL_DB_NAME="${EAZY_SQL_DB_NAME:-EazyMenuDb}"
SQL_APP_USER="${EAZY_SQL_APP_USER:-eazymenu_app}"
SQL_PORT="${EAZY_SQL_PORT:-1433}"
DOTNET_EF_VERSION="${EAZY_DOTNET_EF_VERSION:-9.0.4}"
TLS_ENABLED="${EAZY_ENABLE_TLS:-true}"
WEB_DOMAIN="${EAZY_WEB_DOMAIN:-}"
GIT_REPO="${EAZY_GIT_REPO:-https://github.com/Merkousha/EazyMeny-Main.git}"
GIT_BRANCH="${EAZY_GIT_BRANCH:-}"
CERTBOT_EMAIL="${EAZY_CERTBOT_EMAIL:-}"
APP_ENVIRONMENT="${EAZY_ENVIRONMENT:-Production}"
WEB_PORT="${EAZY_WEB_INTERNAL_PORT:-5000}"

require_env EAZY_SQL_SA_PASSWORD
require_env EAZY_SQL_APP_PASSWORD
if [[ -z "$GIT_REPO" ]]; then
  log FATAL "آدرس مخزن گیت (EAZY_GIT_REPO) باید مشخص شود."; exit 1;
fi

if [[ "$TLS_ENABLED" == "true" ]]; then
  if [[ -z "$WEB_DOMAIN" ]]; then
    log FATAL "برای فعال‌سازی TLS باید متغیر EAZY_WEB_DOMAIN تنظیم شود."; exit 1;
  fi
  if [[ -z "$CERTBOT_EMAIL" ]]; then
    log FATAL "برای دریافت گواهی TLS آدرس ایمیل (EAZY_CERTBOT_EMAIL) لازم است."; exit 1;
  fi
fi

SQL_SA_PASSWORD="$EAZY_SQL_SA_PASSWORD"
SQL_APP_PASSWORD="$EAZY_SQL_APP_PASSWORD"

create_system_user() {
  if ! id "$APP_USER" &>/dev/null; then
    log INFO "ایجاد کاربر سیستمی ${APP_USER}"
    useradd --system --create-home --shell /usr/sbin/nologin "$APP_USER"
  else
    log INFO "کاربر ${APP_USER} از قبل وجود دارد."
  fi
}

ensure_directories() {
  log INFO "ایجاد ساختار دایرکتوری"
  mkdir -p "$SRC_DIR" "$ENV_DIR" "$LOG_DIR/app" "$LOG_DIR/nginx"

  chown -R "$APP_USER":"$APP_GROUP" "$APP_ROOT"
  chown "$APP_USER":"$APP_GROUP" "$LOG_DIR/app"
  chown www-data:adm "$LOG_DIR/nginx" || true
  chmod 750 "$LOG_DIR/app" "$LOG_DIR/nginx"

  chown root:"$APP_GROUP" "$ENV_DIR"
  chmod 750 "$ENV_DIR"
}

install_docker() {
  if ! command -v docker &>/dev/null; then
    log INFO "نصب Docker CE"
    install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg | gpg --dearmor >/etc/apt/keyrings/docker.gpg
    chmod a+r /etc/apt/keyrings/docker.gpg
    echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" >/etc/apt/sources.list.d/docker.list
    apt-get update
    DEBIAN_FRONTEND=noninteractive apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
  else
    log INFO "Docker از قبل نصب شده است."
  fi
  systemctl enable --now docker
  usermod -aG docker "$APP_USER" || true
}

resolve_git_branch() {
  if [[ -n "$GIT_BRANCH" ]]; then
    if git ls-remote --exit-code --heads "$GIT_REPO" "$GIT_BRANCH" >/dev/null 2>&1; then
      log INFO "استفاده از شاخه مشخص‌شده: ${GIT_BRANCH}"
      return
    fi
  log FATAL "شاخه ${GIT_BRANCH} در مخزن ${GIT_REPO} یافت نشد. مقدار EAZY_GIT_BRANCH را اصلاح کنید."
    exit 1
  fi

  log INFO "تشخیص شاخه پیش‌فرض مخزن ${GIT_REPO}"
  local symref=""
  symref=$(git ls-remote --symref "$GIT_REPO" HEAD 2>/dev/null | awk '/^ref:/ {sub("refs/heads/", "", $2); print $2; exit}' || true)

  if [[ -n "$symref" ]]; then
    GIT_BRANCH="$symref"
  else
    for candidate in main master; do
      if git ls-remote --exit-code --heads "$GIT_REPO" "$candidate" >/dev/null 2>&1; then
        GIT_BRANCH="$candidate"
        break
      fi
    done
  fi

  if [[ -z "$GIT_BRANCH" ]]; then
  log FATAL "عدم توانایی در تشخیص شاخه مخزن؛ لطفاً EAZY_GIT_BRANCH را به‌صورت صریح مشخص کنید."
    exit 1
  fi

  log INFO "شاخه تشخیص داده‌شده: ${GIT_BRANCH}"
}

clone_or_update_repo() {
  resolve_git_branch
  if [[ ! -d "$SRC_DIR/.git" ]]; then
    log INFO "کلون کردن مخزن ${GIT_REPO} (شاخه ${GIT_BRANCH})"
    sudo -u "$APP_USER" -H git clone --depth 1 --branch "$GIT_BRANCH" "$GIT_REPO" "$SRC_DIR"
  else
    log INFO "به‌روزرسانی مخزن موجود (شاخه ${GIT_BRANCH})"
    sudo -u "$APP_USER" -H git -C "$SRC_DIR" fetch --all --prune
    if ! sudo -u "$APP_USER" -H git -C "$SRC_DIR" rev-parse --verify "origin/${GIT_BRANCH}" >/dev/null 2>&1; then
  log FATAL "شاخه origin/${GIT_BRANCH} در مخزن یافت نشد. لطفاً EAZY_GIT_BRANCH را به مقدار صحیح تنظیم کنید."
      exit 1
    fi
    sudo -u "$APP_USER" -H git -C "$SRC_DIR" checkout "$GIT_BRANCH"
    sudo -u "$APP_USER" -H git -C "$SRC_DIR" pull --ff-only
  fi
}

run_migrations() {
  log INFO "اجرای مایگریشن‌های پایگاه‌داده در کانتینر"
  local conn="Server=${SQL_CONTAINER_NAME},1433;Database=${SQL_DB_NAME};User Id=${SQL_APP_USER};Password=${SQL_APP_PASSWORD};Encrypt=True;TrustServerCertificate=True;"
  
  log INFO "تست اتصال شبکه‌ای به SQL Server"
  if ! sudo -u "$APP_USER" -H docker compose -f "$COMPOSE_FILE" --env-file "$COMPOSE_ENV_FILE" run --rm --entrypoint bash migrator -c "timeout 5 bash -c 'cat < /dev/null > /dev/tcp/${SQL_CONTAINER_NAME}/1433' && echo 'Network connection to ${SQL_CONTAINER_NAME}:1433 OK'"; then
    log FATAL "عدم دسترسی شبکه‌ای به کانتینر SQL از migrator"
    exit 1
  fi
  
  local ef_commands="set -Eeuo pipefail
echo 'Connection string being used: Server=${SQL_CONTAINER_NAME},1433;Database=${SQL_DB_NAME};User Id=${SQL_APP_USER};Password=***;Encrypt=True;TrustServerCertificate=True;'
dotnet tool list --global | grep -q 'dotnet-ef' || dotnet tool install --global dotnet-ef --version ${DOTNET_EF_VERSION}
export PATH=\"\$PATH:/root/.dotnet/tools\"
dotnet restore src/EazyMenu.Web/EazyMenu.Web.csproj
dotnet ef database update --project src/EazyMenu.Infrastructure/EazyMenu.Infrastructure.csproj --startup-project src/EazyMenu.Web/EazyMenu.Web.csproj --context EazyMenu.Infrastructure.Data.ApplicationDbContext"

  sudo -u "$APP_USER" -H env ConnectionStrings__DefaultConnection="${conn}" \
    docker compose -f "$COMPOSE_FILE" --env-file "$COMPOSE_ENV_FILE" run --rm --entrypoint bash migrator -c "$ef_commands"
}

generate_compose_env_file() {
  log INFO "ایجاد فایل محیطی برای Docker Compose"
  if [[ ! -d "$COMPOSE_DIR" ]]; then
    log FATAL "پیکربندی Docker Compose در مخزن یافت نشد."; exit 1;
  fi
  if [[ ! -f "$COMPOSE_FILE" ]]; then
    log FATAL "فایل Docker Compose (${COMPOSE_FILE}) یافت نشد."; exit 1;
  fi

  cat >"$COMPOSE_ENV_FILE" <<EOF
EAZY_SQL_SA_PASSWORD=${SQL_SA_PASSWORD}
EAZY_SQL_APP_PASSWORD=${SQL_APP_PASSWORD}
SQL_DB_NAME=${SQL_DB_NAME}
SQL_APP_USER=${SQL_APP_USER}
EAZY_ENVIRONMENT=${APP_ENVIRONMENT}
WEB_PORT=${WEB_PORT}
SQL_CONTAINER_NAME=${SQL_CONTAINER_NAME}
SQL_VOLUME_NAME=${SQL_VOLUME_NAME}
SQL_PORT=${SQL_PORT}
HOST_PROJECT_DIR=${APP_ROOT}
HOST_SRC_DIR=${SRC_DIR}
MIGRATIONS_CONNECTION_STRING=Server=${SQL_CONTAINER_NAME},1433;Database=${SQL_DB_NAME};User Id=${SQL_APP_USER};Password=${SQL_APP_PASSWORD};Encrypt=True;TrustServerCertificate=True;
EOF

  chown "$APP_USER":"$APP_GROUP" "$COMPOSE_ENV_FILE"
  chmod 600 "$COMPOSE_ENV_FILE"
}

persist_runtime_configuration() {
  log INFO "ثبت تنظیمات محیطی در ${ENV_DIR}"
  local runtime_file="${ENV_DIR}/runtime.env"

  cat >"$runtime_file" <<EOF
# EazyMenu provisioning snapshot (خودکار تولید شده - شامل رمزها نیست)
APP_ROOT=${APP_ROOT}
SRC_DIR=${SRC_DIR}
COMPOSE_FILE=${COMPOSE_FILE}
APP_ENVIRONMENT=${APP_ENVIRONMENT}
TLS_ENABLED=${TLS_ENABLED}
WEB_DOMAIN=${WEB_DOMAIN}
WEB_PORT=${WEB_PORT}
SQL_DB_NAME=${SQL_DB_NAME}
SQL_APP_USER=${SQL_APP_USER}
SQL_CONTAINER_NAME=${SQL_CONTAINER_NAME}
SQL_VOLUME_NAME=${SQL_VOLUME_NAME}
SQL_PORT=${SQL_PORT}
GIT_REPO=${GIT_REPO}
GIT_BRANCH=${GIT_BRANCH}
DOTNET_EF_VERSION=${DOTNET_EF_VERSION}
EOF

  chown root:"$APP_GROUP" "$runtime_file"
  chmod 640 "$runtime_file"
}

validate_eazymenu_repo_layout() {
  log INFO "اعتبارسنجی ساختار مخزن EazyMenu"
  local required_paths=(
    "$SRC_DIR/EazyMenu.sln"
    "$COMPOSE_FILE"
    "$COMPOSE_DIR/web/Dockerfile"
    "$SRC_DIR/src/EazyMenu.Web/EazyMenu.Web.csproj"
    "$SRC_DIR/src/EazyMenu.Infrastructure/EazyMenu.Infrastructure.csproj"
    "$SRC_DIR/src/EazyMenu.Application/EazyMenu.Application.csproj"
    "$SRC_DIR/src/EazyMenu.Domain/EazyMenu.Domain.csproj"
  )

  for path in "${required_paths[@]}"; do
    if [[ ! -e "$path" ]]; then
  log FATAL "مسیر مورد انتظار ${path} یافت نشد. ساختار مخزن با EazyMenu سازگار نیست.";
      exit 1
    fi
  done
}

start_sql_service() {
  log INFO "راه‌اندازی سرویس SQL Server با Docker Compose"

  local sql_already_running="false"
  local existing_container_id=""
  existing_container_id=$(sudo -u "$APP_USER" -H docker compose -f "$COMPOSE_FILE" --env-file "$COMPOSE_ENV_FILE" ps -q sqlserver || true)

  if [[ -n "$existing_container_id" ]]; then
    sql_already_running="true"
    log INFO "کانتینر SQL Server از قبل در حال اجراست (ID: ${existing_container_id})."
  else
    docker volume create "$SQL_VOLUME_NAME" >/dev/null
    sudo -u "$APP_USER" -H docker compose -f "$COMPOSE_FILE" --env-file "$COMPOSE_ENV_FILE" up -d sqlserver
  fi

  log INFO "منتظر آماده شدن SQL Server"
  local attempt
  for attempt in {1..30}; do
    if docker compose -f "$COMPOSE_FILE" --env-file "$COMPOSE_ENV_FILE" exec -T \
      -e SA_PASSWORD="$SQL_SA_PASSWORD" sqlserver bash -lc '
set -Eeuo pipefail
if [[ -x /opt/mssql-tools18/bin/sqlcmd ]]; then
  SQLCMD="/opt/mssql-tools18/bin/sqlcmd"
elif [[ -x /opt/mssql-tools/bin/sqlcmd ]]; then
  SQLCMD="/opt/mssql-tools/bin/sqlcmd"
else
  exit 127
fi
"$SQLCMD" -C -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1"
' &>/dev/null; then
      log INFO "SQL Server آماده است."
      break
    fi
    sleep 3
    if [[ $attempt -eq 30 ]]; then
      if [[ "$sql_already_running" == "true" ]]; then
  log FATAL "عدم توانایی در اتصال به SQL Server موجود؛ به نظر می‌رسد رمز SA در کانتینر فعلی با مقدار EAZY_SQL_SA_PASSWORD متفاوت است. مقدار درست را تنظیم کنید یا کانتینر و ولوم ${SQL_VOLUME_NAME} را حذف کنید."; exit 1;
      fi
      log FATAL "عدم توانایی در اتصال به SQL Server"; exit 1;
    fi
  done

  docker compose -f "$COMPOSE_FILE" --env-file "$COMPOSE_ENV_FILE" exec -T \
    -e SA_PASSWORD="$SQL_SA_PASSWORD" sqlserver bash -lc '
set -Eeuo pipefail
if [[ -x /opt/mssql-tools18/bin/sqlcmd ]]; then
  SQLCMD="/opt/mssql-tools18/bin/sqlcmd"
elif [[ -x /opt/mssql-tools/bin/sqlcmd ]]; then
  SQLCMD="/opt/mssql-tools/bin/sqlcmd"
else
  echo "sqlcmd binary not found inside SQL Server container" >&2
  exit 127
fi
"$SQLCMD" -C -S localhost -U sa -P "$SA_PASSWORD"
' <<SQL
IF DB_ID('${SQL_DB_NAME}') IS NULL
BEGIN
  CREATE DATABASE [${SQL_DB_NAME}];
END;
GO
IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name = '${SQL_APP_USER}')
BEGIN
  CREATE LOGIN [${SQL_APP_USER}] WITH PASSWORD = '${SQL_APP_PASSWORD}', CHECK_POLICY = ON, CHECK_EXPIRATION = ON;
END;
GO
USE [${SQL_DB_NAME}];
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = '${SQL_APP_USER}')
BEGIN
  CREATE USER [${SQL_APP_USER}] FOR LOGIN [${SQL_APP_USER}];
END;
GO
ALTER ROLE db_owner ADD MEMBER [${SQL_APP_USER}];
GO
SQL
}

build_application_images() {
  log INFO "بیلد ایمیج سرویس وب EazyMenu"
  cd "$SRC_DIR/deploy/docker"
  sudo -u "$APP_USER" -H HOST_PROJECT_DIR="$APP_ROOT" HOST_SRC_DIR="$SRC_DIR" docker compose build eazymenu-web
}

start_application_containers() {
  log INFO "راه‌اندازی کانتینر وب EazyMenu"
  cd "$SRC_DIR/deploy/docker"
  sudo -u "$APP_USER" -H docker compose up -d eazymenu-web
}

configure_nginx() {
  log INFO "پیکربندی Nginx به عنوان Reverse Proxy برای EazyMenu"
  local web_conf="/etc/nginx/sites-available/eazymenu-web.conf"
  local web_server_name="${WEB_DOMAIN:-_}"

  cat >"$web_conf" <<EOF
server {
    listen 80;
    server_name ${web_server_name};

    location / {
        proxy_pass http://127.0.0.1:${WEB_PORT};
        proxy_http_version 1.1;
        proxy_set_header Upgrade \$http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host \$host;
        proxy_set_header X-Real-IP \$remote_addr;
        proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto \$scheme;
    }

    access_log ${LOG_DIR}/nginx-web.access.log;
    error_log ${LOG_DIR}/nginx-web.error.log;
}
EOF

  ln -sf "$web_conf" /etc/nginx/sites-enabled/eazymenu-web.conf

  rm -f /etc/nginx/sites-enabled/default || true
  nginx -t
  systemctl reload nginx
}

obtain_certificates() {
  if [[ "$TLS_ENABLED" != "true" ]]; then
    log INFO "دریافت گواهی TLS غیرفعال است."
    return
  fi

  log INFO "بررسی رکورد DNS برای دامنه TLS"
  require_dns_record "$WEB_DOMAIN"

  log INFO "دریافت گواهی Let's Encrypt"
  certbot --nginx --non-interactive --agree-tos --redirect --no-eff-email \
    -m "$CERTBOT_EMAIL" -d "$WEB_DOMAIN" --deploy-hook "systemctl reload nginx"
  systemctl reload nginx
}

harden_ssh_fail2ban() {
  log INFO "فعال‌سازی Fail2ban برای SSH"
  cat >/etc/fail2ban/jail.d/defaults-debian.conf <<'EOF'
[sshd]
enabled = true
port    = ssh
logpath = %(sshd_log)s
backend = systemd
maxretry = 5
findtime = 15m
bantime = 1h
EOF
  systemctl enable --now fail2ban
}

main() {
  update_system_packages
  install_base_packages
  configure_firewall
  create_system_user
  ensure_directories
  install_docker
  clone_or_update_repo
  validate_eazymenu_repo_layout
  generate_compose_env_file
  persist_runtime_configuration
  start_sql_service
  run_migrations
  build_application_images
  start_application_containers
  configure_nginx
  obtain_certificates
  harden_ssh_fail2ban
  log INFO "نصب به پایان رسید. لطفاً لاگ‌ها و وضعیت سرویس‌ها را بررسی کنید."
}

main "$@"

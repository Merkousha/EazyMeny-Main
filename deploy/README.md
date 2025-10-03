# 📦 راهنمای استقرار پروژه EazyMenu

این سند مراحل آماده‌سازی سرور و استقرار سرویس‌های EazyMenu را با استفاده از اسکریپت `provision-ubuntu.sh` و Docker Compose توضیح می‌دهد. تمامی توضیحات به‌صورت گام‌به‌گام ارائه شده تا بتوانید سرویس را روی سرور تازه نصب‌شده اوبونتو راه‌اندازی کنید.

## 1. پیش‌نیازها

- **سیستم‌عامل:** Ubuntu 22.04 LTS (یا نسخه جدیدتر سازگار)
- **دسترسی کاربری:** دسترسی `root` یا کاربر دارای مجوز `sudo`
- **دامنه:** رکورد A/AAAA فعال برای دامنه‌ای که با EazyMenu استفاده می‌کنید (در صورت فعال‌سازی TLS)
- **پورت‌های باز:**
  - 22 (SSH)
  - 80 (HTTP)
  - 443 (HTTPS)
  - 5000 (پورت داخلی پیش‌فرض برنامه – قابل تغییر با متغیر محیطی)
- **نصب نبودن پکیج‌های Docker:** اگر Docker از قبل نصب شده است، ابتدا نسخه‌های قدیمی را حذف یا به‌روزرسانی کنید.

## 2. متغیرهای محیطی ضروری

پیش از اجرای اسکریپت، متغیرهای زیر را در محیط شل تنظیم کنید:

```bash
export EAZY_SQL_SA_PASSWORD="پسورد_قوی_برای_SA"
export EAZY_SQL_APP_PASSWORD="پسورد_قوی_برای_کاربر_برنامه"
```

متغیرهای اختیاری مهم:

| متغیر | مقدار پیش‌فرض | توضیح |
| --- | --- | --- |
| `EAZY_APP_USER` | `eazymenu` | نام کاربر سیستمی که سرویس‌ها زیر آن اجرا می‌شوند |
| `EAZY_APP_ROOT` | `/opt/eazymenu` | مسیر نصب پروژه |
| `EAZY_GIT_REPO` | `https://github.com/Merkousha/EazyMeny-Main.git` | آدرس مخزن گیت |
| `EAZY_GIT_BRANCH` | (تشخیص خودکار) | شاخه مورد استفاده؛ در صورت نیاز مقداردهی کنید |
| `EAZY_ENVIRONMENT` | `Production` | مقدار `ASPNETCORE_ENVIRONMENT` |
| `EAZY_WEB_INTERNAL_PORT` | `5000` | پورتی که Nginx به آن پراکسی می‌کند |
| `EAZY_ENABLE_TLS` | `true` | فعال‌سازی TLS و دریافت گواهی Let’s Encrypt |
| `EAZY_WEB_DOMAIN` | — | نام دامنه؛ در صورت فعال بودن TLS الزامی است |
| `EAZY_CERTBOT_EMAIL` | — | ایمیل معتبر برای Let’s Encrypt |
| `EAZY_SQL_CONTAINER_NAME` | `eazymenu-sql` | نام کانتینر SQL Server |
| `EAZY_SQL_DB_NAME` | `EazyMenuDb` | نام پایگاه‌داده اصلی |
| `EAZY_SQL_APP_USER` | `eazymenu_app` | کاربر برنامه در SQL |

> **نکته امنیتی:** از قرار دادن مقادیر رمز در تاریخچه شل خودداری کنید؛ در صورت نیاز از فایل‌های محیطی موقت یا `systemd drop-in` استفاده کنید.

## 3. اجرای اسکریپت تدارک سرور

1. کلون یا دانلود مخزن روی سرور:
   ```bash
   git clone https://github.com/Merkousha/EazyMeny-Main.git
   cd EazyMeny-Main/deploy
   ```
2. تنظیم متغیرهای محیطی (بخش قبل).
3. اجرای اسکریپت با دسترسی ریشه:
   ```bash
   sudo -E ./provision-ubuntu.sh
   ```
   - گزینه `-E` متغیرهای محیطی فعلی را حفظ می‌کند.
   - اسکریپت به ترتیب مراحل زیر را انجام می‌دهد:
     - به‌روزرسانی سیستم‌عامل و نصب بسته‌های پایه (Docker, Nginx, Certbot و ...)
     - ساخت کاربر سیستمی اختصاصی و دایرکتوری‌های `/opt/eazymenu`
     - کلون/بروزرسانی مخزن با شاخه مشخص شده
     - تولید فایل `.env` برای Docker Compose و ثبت اسنپ‌شات تنظیمات در `/etc/eazymenu/runtime.env`
     - راه‌اندازی SQL Server و ایجاد دیتابیس + کاربر برنامه
     - اجرای مایگریشن‌های EF Core از کانتینر `migrator`
     - بیلد و اجرای کانتینر `eazymenu-web`
     - تنظیم Nginx و در صورت نیاز دریافت گواهی TLS
     - فعال‌سازی Fail2ban برای محافظت SSH

پس از اتمام موفق، سرویس از طریق `http(s)://<دامنه>` در دسترس خواهد بود.

## 4. ساختار Docker Compose

فایل `deploy/docker/docker-compose.yml` سه سرویس ایجاد می‌کند:

- **sqlserver:** دیتابیس SQL Server 2022 با ولوم دائمی
- **eazymenu-web:** برنامه ASP.NET Core (پورت داخلی 8080، به‌صورت پیش‌فرض با Nginx روی 80/443 منتشر می‌شود)
- **migrator:** کانتینر موقتی جهت اجرای دستورات EF Core (در زمان provisioning و هنگام نیاز به مایگریشن جدید)

فایل `.env` تولیدشده شامل اطلاعات اتصال، نام کانتینرها، مسیرها و پورت‌هاست. از ویرایش دستی فایل جز در موارد ضروری خودداری کنید.

## 5. عملیات نگه‌داری و به‌روزرسانی

### 5.1 به‌روزرسانی نسخه برنامه

```bash
sudo -u eazymenu -H git -C /opt/eazymenu/src pull --ff-only
sudo -u eazymenu -H docker compose -f /opt/eazymenu/src/deploy/docker/docker-compose.yml --env-file /opt/eazymenu/src/deploy/docker/.env build eazymenu-web
sudo -u eazymenu -H docker compose -f /opt/eazymenu/src/deploy/docker/docker-compose.yml --env-file /opt/eazymenu/src/deploy/docker/.env up -d eazymenu-web
```

### 5.2 اجرای مجدد مایگریشن‌ها

```bash
sudo -u eazymenu -H docker compose -f /opt/eazymenu/src/deploy/docker/docker-compose.yml --env-file /opt/eazymenu/src/deploy/docker/.env run --rm migrator
```

### 5.3 مشاهده لاگ‌ها

- لاگ برنامه: `/var/log/eazymenu/app` (در صورت Bind-Mount آینده)
- لاگ Nginx: `/var/log/eazymenu/nginx`
- لاگ کانتینرها:
  ```bash
  sudo -u eazymenu -H docker compose -f /opt/eazymenu/src/deploy/docker/docker-compose.yml logs -f
  ```

### 5.4 تمدید گواهی TLS

Certbot به صورت خودکار از طریق `systemd timer` تمدید را انجام می‌دهد. برای اجرای دستی:

```bash
sudo certbot renew --dry-run
```

## 6. عیب‌یابی متداول

| مشکل | راه‌حل پیشنهادی |
| --- | --- |
| عدم اتصال به SQL Server پس از بارگذاری | رمز `SA` ذخیره‌شده در ولوم با مقدار جدید متفاوت است؛ کانتینر و ولوم را حذف کنید یا رمز صحیح را تنظیم کنید. |
| خطای دریافت گواهی از Let’s Encrypt | از تنظیم بودن DNS و باز بودن پورت 80 اطمینان حاصل کنید. |
| خطای `permission denied` در اجرای Docker | بررسی کنید کاربر `eazymenu` در گروه `docker` قرار گرفته باشد (`sudo usermod -aG docker eazymenu` و سپس خروج/ورود مجدد). |
| توقف برنامه پس از ریبوت | دستور `docker compose up -d` را اجرا کنید یا یک سرویس systemd برای اجرای خودکار اضافه کنید. |

## 7. پاک‌سازی یا حذف سرویس‌ها

```bash
sudo -u eazymenu -H docker compose -f /opt/eazymenu/src/deploy/docker/docker-compose.yml --env-file /opt/eazymenu/src/deploy/docker/.env down
sudo docker volume rm eazymenu-sql-data   # در صورت حذف کامل داده‌ها
sudo rm -rf /opt/eazymenu
sudo rm -rf /var/log/eazymenu /etc/eazymenu
```

> **هشدار:** حذف ولوم `eazymenu-sql-data` تمامی داده‌های پایگاه‌داده را از بین می‌برد. قبل از اجرا از پشتیبان‌گیری اطمینان حاصل کنید.

---

با پیروی از این مراحل می‌توانید استقرار EazyMenu را روی سرور جدید به‌سرعت انجام دهید. در صورت نیاز به سناریوهای پیچیده‌تر (مانند خوشه‌بندی یا مانیتورینگ) می‌توانید بخش‌های مرتبط را در اسکریپت یا Compose سفارشی‌سازی کنید.

# syntax=docker/dockerfile:1.6

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first to leverage Docker layer caching
COPY src/EazyMenu.Web/EazyMenu.Web.csproj src/EazyMenu.Web/
COPY src/EazyMenu.Application/EazyMenu.Application.csproj src/EazyMenu.Application/
COPY src/EazyMenu.Domain/EazyMenu.Domain.csproj src/EazyMenu.Domain/
COPY src/EazyMenu.Infrastructure/EazyMenu.Infrastructure.csproj src/EazyMenu.Infrastructure/

RUN dotnet restore src/EazyMenu.Web/EazyMenu.Web.csproj

# Copy the remaining source code
COPY . .

RUN dotnet publish src/EazyMenu.Web/EazyMenu.Web.csproj -c Release -o /app/publish --no-self-contained

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN useradd --create-home --home /home/app --shell /usr/sbin/nologin appuser

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 8080
USER appuser

ENTRYPOINT ["dotnet", "EazyMenu.Web.dll"]

# پروژه EazyMenu - Clean Architecture

پلتفرم SaaS برای رستوران‌ها و کافه‌ها

## 🏗️ معماری

این پروژه با استفاده از **Clean Architecture** پیاده‌سازی شده است.

```
EazyMenu/
├── src/
│   ├── EazyMenu.Domain/           → لایه Domain (Entities, ValueObjects, Enums)
│   ├── EazyMenu.Application/      → لایه Application (Use Cases, Interfaces, DTOs)
│   ├── EazyMenu.Infrastructure/   → لایه Infrastructure (Data Access, External Services)
│   └── EazyMenu.Web/              → لایه Presentation (MVC, Controllers, Views)
```

## 🎯 Stack تکنولوژی

- **.NET Core 9.0** (MVC)
- **Entity Framework Core 9.0**
- **SQL Server 2022**
- **ASP.NET Core Identity**
- **Clean Architecture**

## 📦 پکیج‌های اصلی

### Domain Layer:
- هیچ وابستگی خارجی ندارد

### Application Layer:
- MediatR (CQRS Pattern)
- FluentValidation
- AutoMapper

### Infrastructure Layer:
- Entity Framework Core
- ASP.NET Core Identity
- QRCoder (تولید QR Code)

### Web Layer:
- ASP.NET Core MVC 9.0
- Bootstrap 5 / Tailwind CSS
- SignalR (Real-time)

## 🚀 راه‌اندازی پروژه

### پیش‌نیازها:
- .NET 9.0 SDK
- SQL Server 2022
- Visual Studio 2022 / VS Code / Rider

### مراحل نصب:

1. **Clone پروژه:**
```bash
git clone https://github.com/yourrepo/EazyMenu.git
cd EazyMenu
```

2. **بازیابی Package ها:**
```bash
dotnet restore
```

3. **تنظیم Connection String:**
در `appsettings.json` فایل Web، Connection String را تنظیم کنید:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EazyMenuDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

4. **اجرای Migration:**
```bash
cd src/EazyMenu.Web
dotnet ef database update
```

5. **اجرای پروژه:**
```bash
dotnet run
```

پروژه در آدرس `https://localhost:5001` اجرا می‌شود.

## 📁 ساختار پروژه

### 1. Domain Layer (EazyMenu.Domain)
**مسئولیت:** منطق کسب‌وکار اصلی

```
Domain/
├── Entities/              → موجودیت‌های اصلی (User, Restaurant, Product, Order, ...)
├── ValueObjects/          → اشیاء ارزشی (Money, Address, PhoneNumber)
├── Enums/                 → Enum ها (OrderStatus, SubscriptionPlan, UserRole)
├── Events/                → Domain Events
├── Exceptions/            → استثناهای سفارشی
└── Common/                → کلاس‌های مشترک (BaseEntity, IAggregateRoot)
```

### 2. Application Layer (EazyMenu.Application)
**مسئولیت:** Use Cases و منطق اپلیکیشن

```
Application/
├── Common/
│   ├── Interfaces/        → Interface های Repository و Service
│   ├── Models/            → DTOs و View Models
│   ├── Behaviors/         → Pipeline Behaviors (Validation, Logging)
│   └── Mappings/          → AutoMapper Profiles
├── Features/
│   ├── Authentication/    → Commands و Queries احراز هویت
│   ├── Restaurants/       → مدیریت رستوران‌ها
│   ├── Products/          → مدیریت محصولات
│   ├── Orders/            → مدیریت سفارش‌ها
│   ├── Reservations/      → مدیریت رزروها
│   └── Admin/             → پنل ادمین
└── DependencyInjection.cs → تزریق وابستگی
```

### 3. Infrastructure Layer (EazyMenu.Infrastructure)
**مسئولیت:** پیاده‌سازی دسترسی به داده و سرویس‌های خارجی

```
Infrastructure/
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Configurations/    → Entity Configurations
│   └── Migrations/        → EF Migrations
├── Repositories/          → پیاده‌سازی Repository ها
├── Services/
│   ├── SmsService.cs      → کاوه‌نگار
│   ├── PaymentService.cs  → زرین‌پال
│   └── QRCodeService.cs   → تولید QR Code
├── Identity/              → ASP.NET Core Identity
└── DependencyInjection.cs
```

### 4. Web Layer (EazyMenu.Web)
**مسئولیت:** UI و API

```
Web/
├── Controllers/           → MVC Controllers
├── Views/                 → Razor Views
├── wwwroot/
│   ├── css/              → فایل‌های CSS (بدون Inline)
│   ├── js/               → فایل‌های JavaScript (بدون Inline)
│   ├── images/           → تصاویر
│   └── qrcodes/          → QR Code های تولید شده
├── Areas/
│   ├── Admin/            → پنل ادمین
│   └── Restaurant/       → پنل رستوران
├── Models/               → View Models
├── Filters/              → Action Filters
├── Hubs/                 → SignalR Hubs
├── Program.cs
└── appsettings.json
```

## 🔐 امنیت

- **Authentication:** ASP.NET Core Identity + JWT
- **Authorization:** Role-Based (RBAC) + Policy-Based
- **Password Hashing:** bcrypt
- **HTTPS:** اجباری
- **CSRF Protection:** فعال
- **XSS Prevention:** فعال
- **SQL Injection:** Parameterized Queries

## 📊 دیتابیس

### جداول اصلی:
- **Users** - کاربران سیستم
- **Restaurants** - رستوران‌ها
- **Subscriptions** - اشتراک‌ها
- **Categories** - دسته‌بندی منو
- **Products** - محصولات
- **Orders** - سفارش‌ها
- **OrderItems** - آیتم‌های سفارش
- **Reservations** - رزروها
- **Payments** - پرداخت‌ها
- **Notifications** - اعلان‌ها

## 🧪 تست

```bash
# اجرای تست‌ها
dotnet test

# Coverage Report
dotnet test /p:CollectCoverage=true
```

## 📚 مستندات

- [PRD.md](./Docs/PRD.MD) - سند نیازمندی‌های محصول
- [User Stories](./Docs/UserStories/) - داستان‌های کاربری
- [API Documentation](./Docs/API.md) - مستندات API
- [Database Schema](./Docs/Database.md) - طراحی دیتابیس

## 🤝 مشارکت

لطفاً قبل از ارسال Pull Request، موارد زیر را رعایت کنید:
1. رعایت Clean Architecture
2. نوشتن Unit Test
3. رعایت SOLID Principles
4. Code Review

## 📞 پشتیبانی

- Email: support@eazymenu.ir
- Website: https://eazymenu.ir

## 📄 لایسنس

این پروژه تحت لایسنس MIT منتشر شده است.

---

**نسخه:** 1.0.0  
**آخرین بروزرسانی:** 2 اکتبر 2025

# راهنمای ادامه پروژه EazyMenu

## ✅ کارهای انجام شده

### 1. Architecture Layer
- ✅ **Domain Layer**: تمام Entity ها، Enum ها و Base Class ها
- ✅ **Application Layer**: Interface ها و DependencyInjection
- ✅ **Infrastructure Layer**: 
  - DbContext با Identity
  - Entity Configurations
  - Repository و UnitOfWork
  - سرویس‌های خارجی (SMS, Payment, QRCode)
- ✅ **Web Layer**: Program.cs با تنظیمات Identity و Session

### 2. Packages نصب شده
- Entity Framework Core 9.0
- ASP.NET Core Identity 9.0
- AutoMapper و MediatR
- QRCoder 1.6.0

---

## 🔄 مراحل بعدی (به ترتیب اولویت)

### مرحله 1: Initial Migration و Database Setup
```powershell
cd "d:\Git\EazyMeny-Main\src\EazyMenu.Infrastructure"
dotnet ef migrations add InitialCreate --startup-project ../EazyMenu.Web
dotnet ef database update --startup-project ../EazyMenu.Web
```

### مرحله 2: ساخت Controller ها

#### 2.1. Account Controller (Authentication)
مسیر: `src/EazyMenu.Web/Controllers/AccountController.cs`
- Register
- Login
- Logout
- ForgotPassword
- ResetPassword

#### 2.2. Admin Area Controllers
مسیر: `src/EazyMenu.Web/Areas/Admin/Controllers/`
- DashboardController
- RestaurantController
- CategoryController
- ProductController
- OrderController
- SubscriptionController

#### 2.3. Restaurant Area Controllers
مسیر: `src/EazyMenu.Web/Areas/Restaurant/Controllers/`
- DashboardController (برای صاحب رستوران)
- MenuController
- OrderManagementController
- ReservationController
- QRCodeController

### مرحله 3: ساخت View ها

#### 3.1. Layout و Shared Views
- `Views/Shared/_Layout.cshtml` (لایه اصلی سایت)
- `Views/Shared/_AdminLayout.cshtml` (پنل ادمین)
- `Views/Shared/_RestaurantLayout.cshtml` (پنل رستوران)
- `Views/Shared/_LoginPartial.cshtml`
- `Views/Shared/Error.cshtml`

#### 3.2. Home Views
- `Views/Home/Index.cshtml` (صفحه اصلی لندینگ)
- `Views/Home/About.cshtml`
- `Views/Home/Pricing.cshtml`
- `Views/Home/Contact.cshtml`

#### 3.3. Account Views
- `Views/Account/Register.cshtml`
- `Views/Account/Login.cshtml`
- `Views/Account/ForgotPassword.cshtml`

### مرحله 4: CQRS با MediatR

#### 4.1. Commands
مسیر: `src/EazyMenu.Application/Features/Restaurants/Commands/`
```
CreateRestaurant/
  ├── CreateRestaurantCommand.cs
  ├── CreateRestaurantCommandHandler.cs
  └── CreateRestaurantCommandValidator.cs
```

#### 4.2. Queries
مسیر: `src/EazyMenu.Application/Features/Restaurants/Queries/`
```
GetRestaurantById/
  ├── GetRestaurantByIdQuery.cs
  └── GetRestaurantByIdQueryHandler.cs
```

#### 4.3. سایر Feature ها
- Products (CRUD)
- Orders (Create, Update Status, Get List)
- Categories (CRUD)
- Subscriptions (Create, Upgrade, GetByRestaurant)

### مرحله 5: ViewModels و DTOs

مسیر: `src/EazyMenu.Application/Common/Models/`
```csharp
// مثال
public class RestaurantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Slug { get; set; }
    public string PhoneNumber { get; set; }
    // ...
}
```

### مرحله 6: AutoMapper Profiles

مسیر: `src/EazyMenu.Application/Common/Mappings/MappingProfile.cs`
```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Restaurant, RestaurantDto>();
        CreateMap<CreateRestaurantCommand, Restaurant>();
        // ...
    }
}
```

### مرحله 7: Validation با FluentValidation

```powershell
cd "d:\Git\EazyMeny-Main\src\EazyMenu.Application"
dotnet add package FluentValidation.DependencyInjectionExtensions
```

### مرحله 8: Frontend Assets

#### 8.1. CSS Framework
- Bootstrap 5 یا Tailwind CSS
- فونت‌های فارسی (IRANSans, Vazir)
- CSS سفارشی

#### 8.2. JavaScript
- jQuery (برای AJAX)
- Chart.js (نمودارها در داشبورد)
- کتابخانه QR Scanner (اسکن QR Code)

### مرحله 9: Seeding Data

مسیر: `src/EazyMenu.Infrastructure/Data/ApplicationDbContextSeed.cs`
```csharp
public static class ApplicationDbContextSeed
{
    public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager)
    {
        // ایجاد Admin پیش‌فرض
        // ایجاد Role ها
    }
}
```

### مرحله 10: Testing

#### 10.1. Unit Tests
```
tests/
├── EazyMenu.Application.Tests/
└── EazyMenu.Infrastructure.Tests/
```

#### 10.2. Integration Tests
```
tests/
└── EazyMenu.Web.Tests/
```

---

## 📁 ساختار کامل پروژه

```
src/
├── EazyMenu.Domain/              ✅ کامل شده
│   ├── Common/
│   ├── Entities/
│   └── Enums/
│
├── EazyMenu.Application/         ✅ پایه آماده
│   ├── Common/
│   │   ├── Interfaces/
│   │   ├── Models/               ⬜ باید ساخته شود
│   │   └── Mappings/             ⬜ باید ساخته شود
│   ├── Features/                 ⬜ باید ساخته شود
│   │   ├── Restaurants/
│   │   ├── Products/
│   │   ├── Orders/
│   │   └── Subscriptions/
│   └── DependencyInjection.cs
│
├── EazyMenu.Infrastructure/      ✅ کامل شده
│   ├── Data/
│   ├── Repositories/
│   ├── Services/
│   └── DependencyInjection.cs
│
└── EazyMenu.Web/                 🔄 نیاز به توسعه
    ├── Areas/                    ⬜ باید ساخته شود
    │   ├── Admin/
    │   └── Restaurant/
    ├── Controllers/              ⬜ باید ساخته شود
    ├── Views/                    ⬜ باید ساخته شود
    ├── wwwroot/
    │   ├── css/
    │   ├── js/
    │   └── images/
    ├── appsettings.json          ✅ آماده
    └── Program.cs                ✅ پیکربندی شده
```

---

## 🎯 اولویت‌بندی فیچرها

### Priority 1 (MVP - باید حتماً باشد)
1. ✅ Authentication System
2. ⬜ Restaurant Management (CRUD)
3. ⬜ Category و Product Management
4. ⬜ Order Management (ایجاد و مشاهده)
5. ⬜ QR Code Generation
6. ⬜ Basic Dashboard

### Priority 2 (نسخه بعدی)
1. ⬜ Payment Integration (Zarinpal)
2. ⬜ SMS Integration (Kavenegar)
3. ⬜ Subscription Management
4. ⬜ Reservation System
5. ⬜ Notification System

### Priority 3 (پیشرفته)
1. ⬜ Website Builder
2. ⬜ Analytics Dashboard
3. ⬜ Multi-language Support
4. ⬜ PWA Support
5. ⬜ API Documentation (Swagger)

---

## 🔧 دستورات مفید

### Build و Run
```powershell
cd "d:\Git\EazyMeny-Main\src\EazyMenu.Web"
dotnet build
dotnet run
```

### Migration
```powershell
# اضافه کردن Migration
dotnet ef migrations add [MigrationName] --startup-project ../EazyMenu.Web

# اعمال Migration
dotnet ef database update --startup-project ../EazyMenu.Web

# حذف آخرین Migration
dotnet ef migrations remove --startup-project ../EazyMenu.Web
```

### Package Management
```powershell
# نصب پکیج
dotnet add package [PackageName]

# حذف پکیج
dotnet remove package [PackageName]

# لیست پکیج‌ها
dotnet list package
```

---

## 🚀 مرحله بعدی پیشنهادی

**بهترین گام بعدی:** ایجاد Migration و ساخت Database

```powershell
cd "d:\Git\EazyMeny-Main\src\EazyMenu.Infrastructure"
dotnet ef migrations add InitialCreate --startup-project ../EazyMenu.Web
```

بعد از موفقیت Migration، می‌توانید:
1. AccountController بسازید
2. View های Authentication را اضافه کنید
3. Admin Dashboard را شروع کنید

---

## 📞 نکات مهم

### Connection String
فایل `appsettings.json` را باز کنید و Connection String را مطابق SQL Server خود تنظیم کنید:
```json
"DefaultConnection": "Server=localhost;Database=EazyMenuDB;Trusted_Connection=True;TrustServerCertificate=True"
```

### API Keys
قبل از استفاده از سرویس‌های خارجی، API Key ها را در `appsettings.json` وارد کنید:
- Kavenegar API Key (برای SMS)
- Zarinpal Merchant ID (برای پرداخت)

---

## ✅ بعد از هر Task موفق

پس از اتمام هر وظیفه، **حتماً** این مراحل را انجام دهید:

### 1. بیلد پروژه
```powershell
cd "d:\Git\EazyMeny-Main"
dotnet build EazyMenu.sln
```
مطمئن شوید که پروژه بدون خطا Build می‌شود.

### 2. به‌روزرسانی مستندات

#### 2.1. فایل `Docs/ProgressLog.md`
پیشرفت انجام شده را ثبت کنید:
```markdown
## [تاریخ] - [نام Feature]

### انجام شده:
- ✅ [توضیح کار انجام شده]
- ✅ [فایل‌های ایجاد/تغییر یافته]

### نتیجه:
- Build Status: ✅ موفق
- Tests: [وضعیت تست‌ها]
- Issues: [مشکلات احتمالی]
```

#### 2.2. فایل `Docs/Todo.md`
- Task تکمیل شده را با ✅ علامت بزنید
- Task های جدید کشف شده را اضافه کنید
- اولویت‌ها را در صورت نیاز به‌روز کنید

### 3. Commit تغییرات (اختیاری)
```powershell
git add .
git commit -m "feat: [توضیح مختصر Task]"
```

---

💡 **تمام لایه‌های پایه آماده هستند! اکنون می‌توانید روی فیچرها کار کنید.**

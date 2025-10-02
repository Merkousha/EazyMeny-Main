# 🌱 Database Seeder - راهنمای استفاده

## 📋 داده‌های Seed شده

Seeder به صورت خودکار در محیط Development اجرا می‌شود و داده‌های زیر را ایجاد می‌کند:

### 👥 کاربران (3 نفر)

| نقش | ایمیل | رمز عبور | توضیحات |
|-----|-------|----------|---------|
| **Admin** | `admin@eazymenu.ir` | `Admin@123` | دسترسی کامل به پنل مدیریت |
| **RestaurantOwner** | `owner@restaurant.com` | `Owner@123` | صاحب 3 رستوران |
| **Customer** | `customer@test.com` | `Customer@123` | مشتری عادی |

### 🏪 رستوران‌ها (3 عدد)

1. **رستوران زیتون** (`zeitoon`)
   - غذاهای ایرانی و بین‌المللی
   - سفارش آنلاین ✅
   - رزرو میز ✅
   - هزینه ارسال: 25,000 تومان
   - حداقل سفارش: 50,000 تومان

2. **فست‌فود برگر استار** (`burger-star`)
   - برگر و فست‌فود
   - سفارش آنلاین ✅
   - رزرو میز ❌
   - هزینه ارسال: 20,000 تومان
   - حداقل سفارش: 40,000 تومان

3. **کافه‌رستوران نیلوفر** (`niloofar-cafe`)
   - کافی‌شاپ و دسر
   - سفارش آنلاین ✅
   - رزرو میز ✅
   - هزینه ارسال: 30,000 تومان
   - حداقل سفارش: 60,000 تومان

### 📂 دسته‌بندی‌ها (8 عدد)

**رستوران زیتون:**
- غذاهای ایرانی
- پیش‌غذا
- نوشیدنی

**برگر استار:**
- برگر
- پیتزا
- سیب‌زمینی

**نیلوفر:**
- کافی‌شاپی
- دسر

### 🍽️ محصولات (11 عدد)

**رستوران زیتون:**
- چلوکباب کوبیده - 180,000 تومان
- چلوکباب برگ - 250,000 تومان
- قورمه سبزی - 150,000 تومان

**برگر استار:**
- برگر مخصوص - 120,000 تومان
- چیزبرگر - 90,000 تومان

**کافه نیلوفر:**
- کاپوچینو - 45,000 تومان
- لاته - 40,000 تومان

### 💳 اشتراک (1 عدد)

- کاربر: `owner@restaurant.com`
- پلن: **Standard**
- وضعیت: **Active** (فعال)
- تاریخ شروع: 30 روز پیش
- تاریخ پایان: 60 روز آینده
- قیمت: 500,000 تومان

---

## 🚀 نحوه اجرا

### روش 1: اتوماتیک (پیشنهادی)

Seeder به صورت خودکار در محیط Development هنگام اجرای برنامه فعال می‌شود:

```powershell
cd "d:\Git\EazyMeny-Main\src\EazyMenu.Web"
dotnet run
```

خروجی در Console:
```
✅ Role created: Admin
✅ Role created: RestaurantOwner
✅ Role created: Customer
✅ Admin user created: admin@eazymenu.ir
✅ Owner user created: owner@restaurant.com
✅ Customer user created: customer@test.com
✅ 3 restaurants seeded
✅ 8 categories seeded
✅ 11 products seeded
✅ Subscription seeded for user
✅ Database seeded successfully!
```

### روش 2: دستی (برای Production)

برای غیرفعال کردن Seed اتوماتیک، متغیر محیطی را تغییر دهید:

```json
// appsettings.json
{
  "EnableDatabaseSeeding": false
}
```

یا در `Program.cs`:

```csharp
// Comment out this section:
/*
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await DatabaseSeeder.SeedAsync(services);
    }
}
*/
```

---

## 🔑 ورود به سیستم

### پنل Admin:
1. مراجعه به: http://localhost:5125/Account/Login
2. ایمیل: `admin@eazymenu.ir`
3. رمز عبور: `Admin@123`
4. بعد از ورود: http://localhost:5125/Admin/Restaurant

### پنل صاحب رستوران:
1. مراجعه به: http://localhost:5125/Account/Login
2. ایمیل: `owner@restaurant.com`
3. رمز عبور: `Owner@123`
4. بعد از ورود: می‌توانید 3 رستوران خود را مدیریت کنید

### حساب مشتری:
1. مراجعه به: http://localhost:5125/Account/Login
2. ایمیل: `customer@test.com`
3. رمز عبور: `Customer@123`
4. برای سفارش آنلاین

---

## 🔄 Reseed Database

اگر می‌خواهید دیتابیس را از نو Seed کنید:

```powershell
# حذف دیتابیس
cd "d:\Git\EazyMeny-Main\src\EazyMenu.Infrastructure"
dotnet ef database drop --startup-project ../EazyMenu.Web --force

# ایجاد مجدد و Seed
cd ../EazyMenu.Web
dotnet run
```

یا از SQL Server Management Studio:
```sql
USE master;
GO
DROP DATABASE [eazy-menu];
GO
```

سپس برنامه را Run کنید، دیتابیس و داده‌ها خودکار ایجاد می‌شوند.

---

## 📝 نکات مهم

### ✅ چه زمانی Seed اجرا می‌شود؟
- فقط در محیط **Development**
- فقط اگر داده قبلی وجود نداشته باشد (Idempotent)
- هر بار Run برنامه

### ⚠️ هشدارها
- **هرگز در Production فعال نکنید!**
- رمزهای عبور Seed شده فقط برای تست است
- داده‌های واقعی کاربران را Seed نکنید

### 🔒 امنیت
- رمزهای عبور با Identity HashPassword ذخیره می‌شوند
- EmailConfirmed و PhoneNumberConfirmed روی `true` است
- همه کاربران `IsActive = true` دارند

---

## 🛠️ سفارشی‌سازی

برای اضافه کردن داده‌های بیشتر، فایل را ویرایش کنید:

```
src/EazyMenu.Infrastructure/Data/DatabaseSeeder.cs
```

### مثال: اضافه کردن رستوران جدید

```csharp
new Restaurant
{
    Id = Guid.NewGuid(),
    Name = "رستوران شما",
    Slug = "your-restaurant",
    OwnerId = ownerId,
    // ...
}
```

---

## 📊 آمار داده‌های Seed شده

```
👥 Users:        3
🏪 Restaurants:  3
📂 Categories:   8
🍽️ Products:    11
💳 Subscriptions: 1
───────────────────
📦 Total:        26 records
```

---

## ✅ تست Checklist

بعد از Seed و Run:

- [ ] ورود با حساب Admin موفق است
- [ ] ورود با حساب Owner موفق است
- [ ] لیست 3 رستوران در `/Admin/Restaurant` نمایش داده می‌شود
- [ ] Owner فقط رستوران‌های خودش را می‌بیند
- [ ] Admin همه رستوران‌ها را می‌بیند
- [ ] دسته‌بندی‌ها برای هر رستوران صحیح است
- [ ] محصولات در دسته‌بندی‌های صحیح هستند
- [ ] اشتراک Owner فعال است

---

**📅 آخرین بروزرسانی:** 2025-10-03  
**🔖 نسخه Seeder:** 1.0  
**👨‍💻 توسعه:** EazyMenu Team

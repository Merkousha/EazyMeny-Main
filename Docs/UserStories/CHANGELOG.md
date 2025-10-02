# 📋 خلاصه تغییرات - سازماندهی User Stories

## 🎯 هدف
سازماندهی User Story ها بر اساس **کیس‌های بیزنسی** برای دسترسی و مدیریت بهتر

---

## 🗂️ ساختار قبل

```
UserStories/
├── UserStory01.md
├── UserStory02.md
├── UserStory03.md
├── UserStory04.md
├── UserStory05.md
├── UserStory06.md
├── UserStory07.md
├── UserStory08.md
├── UserStory09.md
├── UserStory10.md
├── UserStory11.md
├── UserStory12.md
├── UserStory13.md
├── UserStory14.md
└── UserStory15.md
```

**مشکلات:**
- ❌ سخت‌تر برای یافتن User Story مورد نظر
- ❌ فقدان دسته‌بندی منطقی
- ❌ عدم ارتباط واضح با کیس‌های بیزنسی
- ❌ سخت‌تر برای کار تیمی

---

## 🗂️ ساختار جدید (بهینه شده)

```
UserStories/
├── README.md (راهنمای اصلی)
├── INDEX.md (نقشه کلی و جدول)
│
├── 01-Authentication/ (احراز هویت)
│   ├── README.md
│   ├── US-001-Registration.md
│   ├── US-002-Login.md
│   └── US-003-PasswordRecovery.md
│
├── 02-Subscription/ (مدیریت اشتراک)
│   ├── README.md
│   ├── US-004-PurchaseSubscription.md
│   └── US-005-ManageSubscription.md
│
├── 03-MenuManagement/ (مدیریت منو)
│   ├── README.md
│   ├── US-006-ManageCategories.md
│   ├── US-007-ManageProducts.md
│   └── US-008-QRCodeManagement.md
│
├── 04-OrderSystem/ (سیستم سفارش)
│   ├── README.md
│   ├── US-009-CustomerOrder.md
│   └── US-010-RestaurantOrderManagement.md
│
├── 05-ReservationSystem/ (سیستم رزرو)
│   ├── README.md
│   └── US-011-TableReservation.md
│
├── 06-WebsiteBuilder/ (سازنده وب‌سایت)
│   ├── README.md
│   └── US-012-WebsiteBuilder.md
│
├── 07-AdminPanel/ (پنل ادمین)
│   ├── README.md
│   ├── US-013-AdminDashboard.md
│   └── US-014-AdminReporting.md
│
└── 08-Notifications/ (سیستم اعلان‌ها)
    ├── README.md
    └── US-015-NotificationSystem.md
```

---

## ✨ مزایای ساختار جدید

### 1. **دسترسی آسان‌تر**
- ✅ یافتن سریع User Story بر اساس کیس بیزنسی
- ✅ نام‌گذاری معنادار (به جای شماره خشک)
- ✅ فایل README در هر پوشه برای راهنمایی

### 2. **مدیریت بهتر تیمی**
- ✅ هر تیم روی یک کیس بیزنسی مشخص کار می‌کند
- ✅ کاهش Conflict در Git
- ✅ Code Review راحت‌تر

### 3. **مستندسازی بهتر**
- ✅ README اختصاصی برای هر دسته
- ✅ INDEX.md برای نمای کلی
- ✅ لینک‌دهی آسان بین اسناد

### 4. **مقیاس‌پذیری**
- ✅ افزودن US جدید در پوشه مربوطه
- ✅ افزودن کیس بیزنسی جدید
- ✅ نگهداری راحت‌تر

---

## 📊 آمار تغییرات

| مورد | تعداد |
|------|-------|
| تعداد کل User Stories | 15 |
| تعداد پوشه‌های ایجاد شده | 8 |
| تعداد فایل README | 9 (1 اصلی + 8 پوشه) |
| فایل‌های جابجا شده | 15 |
| فایل‌های تغییر نام یافته | 15 |

---

## 🗺️ نقشه کیس‌های بیزنسی

| # | کیس بیزنسی | پوشه | تعداد US |
|---|------------|------|----------|
| 1 | احراز هویت | `01-Authentication` | 3 |
| 2 | مدیریت اشتراک | `02-Subscription` | 2 |
| 3 | مدیریت منو | `03-MenuManagement` | 3 |
| 4 | سیستم سفارش | `04-OrderSystem` | 2 |
| 5 | سیستم رزرو | `05-ReservationSystem` | 1 |
| 6 | وب‌سایت‌ساز | `06-WebsiteBuilder` | 1 |
| 7 | پنل ادمین | `07-AdminPanel` | 2 |
| 8 | اعلان‌ها | `08-Notifications` | 1 |

---

## 📝 قوانین نام‌گذاری جدید

### فرمت نام فایل:
```
US-[شماره]-[نام معنادار].md
```

### مثال‌ها:
- ✅ `US-001-Registration.md` (خوب)
- ✅ `US-009-CustomerOrder.md` (خوب)
- ❌ `UserStory01.md` (قدیمی)
- ❌ `Story1.md` (بد)

### فرمت نام پوشه:
```
[شماره]-[نام کیس بیزنسی]
```

### مثال‌ها:
- ✅ `01-Authentication` (خوب)
- ✅ `04-OrderSystem` (خوب)
- ❌ `Auth` (بد - خیلی کوتاه)
- ❌ `authentication-module` (بد - بدون شماره)

---

## 🔗 لینک‌های مهم

| سند | مسیر |
|-----|------|
| فهرست اصلی | [UserStories/README.md](./README.md) |
| نقشه کلی | [UserStories/INDEX.md](./INDEX.md) |
| احراز هویت | [01-Authentication/README.md](./01-Authentication/README.md) |
| مدیریت اشتراک | [02-Subscription/README.md](./02-Subscription/README.md) |
| مدیریت منو | [03-MenuManagement/README.md](./03-MenuManagement/README.md) |
| سیستم سفارش | [04-OrderSystem/README.md](./04-OrderSystem/README.md) |
| سیستم رزرو | [05-ReservationSystem/README.md](./05-ReservationSystem/README.md) |
| وب‌سایت‌ساز | [06-WebsiteBuilder/README.md](./06-WebsiteBuilder/README.md) |
| پنل ادمین | [07-AdminPanel/README.md](./07-AdminPanel/README.md) |
| اعلان‌ها | [08-Notifications/README.md](./08-Notifications/README.md) |

---

## 📚 راهنمای استفاده

### برای یافتن یک User Story:

**روش 1: از طریق کیس بیزنسی**
1. شناسایی کیس بیزنسی (مثلاً "سفارش")
2. باز کردن پوشه مربوطه (`04-OrderSystem/`)
3. خواندن README پوشه
4. باز کردن User Story مورد نظر

**روش 2: از طریق INDEX**
1. باز کردن `INDEX.md`
2. جستجو با Ctrl+F
3. کلیک روی لینک User Story

**روش 3: جستجوی مستقیم**
- جستجو در VS Code: `Ctrl+P` → `US-009`
- یا جستجو در محتوا: `Ctrl+Shift+F`

---

## 🎯 نتیجه‌گیری

✅ ساختار جدید:
- **سازمان‌یافته‌تر**
- **قابل فهم‌تر**
- **قابل نگهداری‌تر**
- **مناسب برای کار تیمی**

---

## 🔄 تاریخچه تغییرات

| تاریخ | نسخه | تغییرات | مسئول |
|-------|------|---------|-------|
| 2025-10-02 | 2.0 | سازماندهی بر اساس کیس‌های بیزنسی | Product Team |
| 2025-10-02 | 1.0 | ایجاد اولیه User Stories | Product Team |

---

**توجه:** این ساختار استاندارد است و تمام اعضای تیم باید از آن پیروی کنند.

---

**آخرین بروزرسانی:** 2 اکتبر 2025  
**نسخه:** 2.0

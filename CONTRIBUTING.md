# 🤝 راهنمای مشارکت در EazyMenu

خوش آمدید! ما خوشحالیم که می‌خواهید در توسعه EazyMenu مشارکت کنید. این راهنما به شما کمک می‌کند تا به بهترین شکل در پروژه مشارکت داشته باشید.

---

## 📋 فهرست مطالب

1. [روش‌های مشارکت](#روش‌های-مشارکت)
2. [راه‌اندازی محیط توسعه](#راه‌اندازی-محیط-توسعه)
3. [استانداردهای کدنویسی](#استانداردهای-کدنویسی)
4. [فرآیند Pull Request](#فرآیند-pull-request)
5. [گزارش باگ](#گزارش-باگ)
6. [پیشنهاد ویژگی جدید](#پیشنهاد-ویژگی-جدید)
7. [Code of Conduct](#code-of-conduct)

---

## 🎯 روش‌های مشارکت

### 1. گزارش باگ 🐛
اگر باگی پیدا کردید، آن را در [GitHub Issues](https://github.com/Merkousha/EazyMenu/issues) گزارش دهید.

### 2. پیشنهاد ویژگی جدید 💡
ایده جدیدی دارید؟ در [GitHub Discussions](https://github.com/Merkousha/EazyMenu/discussions) مطرح کنید.

### 3. بهبود مستندات 📚
مستندات را بهتر کنید، مثال‌های بیشتری اضافه کنید، یا اشتباهات املایی را اصلاح کنید.

### 4. کد بنویسید 💻
باگ‌ها را برطرف کنید یا ویژگی‌های جدید پیاده‌سازی کنید.

### 5. بررسی Pull Request‌ها 👀
از دیگران کمک بگیرید و Pull Request‌های آن‌ها را بررسی کنید.

### 6. ترجمه 🌍
پروژه را به زبان‌های دیگر ترجمه کنید.

---

## 🛠️ راه‌اندازی محیط توسعه

### پیش‌نیازها:
- .NET 9.0 SDK
- SQL Server 2022 (یا LocalDB)
- Visual Studio 2022 / VS Code
- Git

### مراحل نصب:

```bash
# 1. Fork کردن repository
# در GitHub روی دکمه Fork کلیک کنید

# 2. Clone کردن
git clone https://github.com/[your-username]/EazyMenu.git
cd EazyMenu

# 3. اضافه کردن upstream
git remote add upstream https://github.com/[main-repo]/EazyMenu.git

# 4. نصب dependencies
dotnet restore

# 5. ساخت دیتابیس
cd src/EazyMenu.Infrastructure
dotnet ef database update --startup-project ../EazyMenu.Web

# 6. اجرای پروژه
cd ../EazyMenu.Web
dotnet run
```

---

## 📝 استانداردهای کدنویسی

### 1. معماری Clean Architecture
- **Domain**: بدون وابستگی به لایه‌های دیگر
- **Application**: CQRS با MediatR
- **Infrastructure**: پیاده‌سازی repository و services
- **Web**: MVC (بدون Razor Pages)

### 2. نام‌گذاری
```csharp
// Classes/Interfaces: PascalCase
public class RestaurantService
public interface IRepository<T>

// Methods: PascalCase
public async Task<Result> CreateRestaurantAsync()

// Variables/Parameters: camelCase
var restaurantId = Guid.NewGuid();
string userName = "test";

// Private fields: _camelCase
private readonly IMediator _mediator;

// Constants: PascalCase
public const string DefaultLanguage = "fa";
```

### 3. زبان
- **کد**: انگلیسی (Classes, Methods, Variables)
- **کامنت‌های توضیحی**: فارسی
- **UI**: فارسی

### 4. Async/Await
همیشه از `async/await` استفاده کنید و نام متدها با `Async` تمام شود:

```csharp
public async Task<Restaurant> GetRestaurantByIdAsync(Guid id, CancellationToken cancellationToken)
{
    return await _repository.GetByIdAsync(id, cancellationToken);
}
```

### 5. استفاده از CancellationToken
در تمام متدهای async از `CancellationToken` استفاده کنید.

### 6. Validation
از FluentValidation برای اعتبارسنجی استفاده کنید:

```csharp
public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام رستوران الزامی است")
            .MaximumLength(200).WithMessage("نام رستوران نباید بیشتر از 200 کاراکتر باشد");
    }
}
```

### 7. Logging
از ILogger برای لاگ‌گیری استفاده کنید:

```csharp
_logger.LogInformation("Creating restaurant with name {RestaurantName}", request.Name);
_logger.LogError(ex, "Error creating restaurant {RestaurantId}", restaurantId);
```

---

## 🔄 فرآیند Pull Request

### 1. برانچ جدید بسازید
```bash
git checkout -b feature/add-reservation-system
# یا
git checkout -b fix/menu-display-bug
```

### 2. تغییرات را Commit کنید
```bash
git add .
git commit -m "feat: اضافه کردن سیستم رزرو میز"
```

**قالب Commit Message:**
- `feat:` ویژگی جدید
- `fix:` رفع باگ
- `docs:` تغییرات مستندات
- `style:` تغییرات formatting (بدون تغییر منطق)
- `refactor:` بازنویسی کد
- `test:` اضافه کردن تست
- `chore:` تغییرات build یا ابزار

### 3. Push کنید
```bash
git push origin feature/add-reservation-system
```

### 4. Pull Request بسازید
- عنوان واضح و مشخص
- توضیحات کامل از تغییرات
- لینک به Issue مرتبط (اگر هست)
- اسکرین‌شات (برای تغییرات UI)

### 5. بررسی و Merge
- منتظر Review بمانید
- تغییرات درخواستی را اعمال کنید
- بعد از تأیید، Merge می‌شود

---

## 🐛 گزارش باگ

برای گزارش باگ، یک Issue جدید با این اطلاعات ایجاد کنید:

### الگوی گزارش باگ:

```markdown
**توضیح باگ:**
توضیح واضح و مختصر از باگ

**مراحل بازتولید:**
1. برو به '...'
2. کلیک روی '...'
3. اسکرول کن به '...'
4. خطا را ببین

**رفتار مورد انتظار:**
توضیح دهید چه چیزی باید اتفاق بیفتد

**اسکرین‌شات:**
اگر امکان دارد، اسکرین‌شات اضافه کنید

**محیط:**
- OS: [مثلاً Windows 11]
- Browser: [مثلاً Chrome 120]
- Version: [مثلاً v1.2.0]

**اطلاعات اضافی:**
هر اطلاعات دیگری که مفید باشد
```

---

## 💡 پیشنهاد ویژگی جدید

برای پیشنهاد ویژگی جدید:

### الگوی پیشنهاد:

```markdown
**مشکل یا نیاز:**
توضیح دهید چه مشکلی را این ویژگی حل می‌کند

**راه‌حل پیشنهادی:**
توضیحات واضح از ویژگی پیشنهادی

**راه‌حل‌های جایگزین:**
آیا راه‌حل‌های دیگری وجود دارد؟

**مثال‌ها:**
لینک یا مثال از پیاده‌سازی‌های مشابه

**اولویت:**
کم / متوسط / بالا

**پیاده‌سازی:**
آیا مایلید خودتان آن را پیاده‌سازی کنید؟
```

---

## 🧪 تست

قبل از ارسال Pull Request:

```bash
# اجرای تست‌ها
dotnet test

# بررسی Build
dotnet build

# بررسی Warnings
dotnet build /p:TreatWarningsAsErrors=true
```

---

## 📋 Checklist قبل از Pull Request

- [ ] کد با استانداردهای پروژه مطابقت دارد
- [ ] تست‌های مرتبط نوشته شده
- [ ] مستندات به‌روزرسانی شده
- [ ] Commit messages واضح هستند
- [ ] تمام تست‌ها پاس می‌شوند
- [ ] بدون Warning کامپایل می‌شود
- [ ] تغییرات UI در موبایل تست شده

---

## 📜 Code of Conduct

### احترام و شایستگی
- با احترام با همه رفتار کنید
- به نظرات دیگران احترام بگذارید
- از زبان نامناسب خودداری کنید
- تمرکز روی بهبود پروژه باشد

### حرفه‌ای بودن
- انتقاد سازنده ارائه دهید
- منطقی و مستدل بحث کنید
- از حملات شخصی خودداری کنید

### همکاری
- دیگران را یاری کنید
- دانش خود را به اشتراک بگذارید
- از تجربیات یکدیگر یاد بگیرید

---

## 🎁 تقدیر از مشارکت‌کنندگان

همه مشارکت‌کنندگان در فایل [CONTRIBUTORS.md](CONTRIBUTORS.md) ذکر می‌شوند.

---

## 📞 ارتباط با ما

- 💬 **Discord**: [لینک دیسکورد]
- 📧 **Email**: contribute@eazymenu.ir
- 🐦 **Twitter**: [@EazyMenuIO]
- 💻 **GitHub Discussions**: [لینک Discussions]

---

## 🏆 مشارکت‌کنندگان برتر

<!-- این بخش به صورت خودکار به‌روزرسانی می‌شود -->

---

**با تشکر از مشارکت شما! 🙏**

هر کمک کوچک یا بزرگی ارزشمند است. با هم EazyMenu را بهترین سیستم مدیریت رستوران Open Source می‌سازیم! 🚀

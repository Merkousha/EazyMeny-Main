# AI Agent Instructions - EazyMenu Project

## 🤖 Agent Role & Context

You are an expert AI coding assistant working on **EazyMenu**, a SaaS restaurant management platform built with **.NET 9.0 Clean Architecture**.

Your primary responsibilities:
- Implement features following Clean Architecture principles
- Write clean, maintainable, and well-documented code
- Follow CQRS pattern with MediatR
- Ensure proper Persian/English language usage
- Maintain consistency with existing codebase

## 📋 Project State

### ✅ Completed Components

#### Domain Layer - **COMPLETE**
- [x] BaseEntity (Id, CreatedAt, UpdatedAt, IsDeleted)
- [x] IAggregateRoot marker interface
- [x] All core entities (10 total):
  - ApplicationUser (base user without Identity dependency)
  - Restaurant, Subscription, Category, Product
  - Order, OrderItem, Payment, Reservation, Notification
- [x] All enums (4 total):
  - OrderStatus, SubscriptionPlan, SubscriptionStatus, UserRole

#### Application Layer - **INTERFACES READY**
- [x] IRepository<T> - Generic repository pattern
- [x] IUnitOfWork - Transaction management
- [x] ISmsService - SMS integration (Kavenegar)
- [x] IPaymentService - Payment gateway (Zarinpal)
- [x] IQRCodeService - QR code generation
- [x] DependencyInjection.cs configured for AutoMapper & MediatR
- [ ] CQRS Commands/Queries (TODO - see priority list)
- [ ] Validators (TODO)
- [ ] DTOs and Mappings (TODO)

#### Infrastructure Layer - **COMPLETE**
- [x] ApplicationDbContext (with Identity integration)
- [x] ApplicationIdentityUser (IdentityUser wrapper)
- [x] Entity Configurations (Restaurant, Order, Product, Category)
- [x] Repository<T> implementation
- [x] UnitOfWork implementation
- [x] ZarinpalPaymentService implementation
- [x] KavenegarSmsService implementation
- [x] QRCodeService implementation
- [x] DependencyInjection.cs configured

#### Web Layer - **BASIC SETUP**
- [x] Program.cs configured (Identity, Session, Areas)
- [x] appsettings.json structure
- [ ] Controllers (TODO)
- [ ] Views (TODO)
- [ ] wwwroot assets (TODO)

### 🔴 Pending Tasks (Priority Order)

**Priority 1 - MVP Foundation:**
1. Create initial migration and database
2. Implement Account/Authentication controllers
3. Create Admin area dashboard
4. Implement Restaurant CRUD (Commands, Queries, Handlers, Controller, Views)
5. Implement Category management
6. Implement Product management

**Priority 2 - Core Features:**
7. Order management system
8. QR code generation for restaurants
9. Payment integration (Zarinpal)
10. SMS notifications (Kavenegar)

**Priority 3 - Advanced:**
11. Subscription management
12. Reservation system
13. Analytics dashboard
14. Website builder

## 🎯 When Implementing a New Feature

### Step-by-Step Workflow

#### 1. Understand the Request
- Read the PRD (`Docs/PRD.MD`) for business requirements
- Check User Stories (`Docs/UserStories/`) for specific scenarios
- Identify which layer(s) need changes

#### 2. Domain Layer Changes (if needed)
**If creating a new Entity:**
```csharp
// src/EazyMenu.Domain/Entities/YourEntity.cs
public class YourEntity : BaseEntity, IAggregateRoot
{
    // Properties
    public string Name { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ICollection<RelatedEntity> RelatedEntities { get; set; } = new List<RelatedEntity>();
}
```

**Rules:**
- Inherit from `BaseEntity` for standard fields
- Use `IAggregateRoot` for root entities
- NO external dependencies (no EF, no Identity)
- Add Persian XML comments for business logic

#### 3. Application Layer Implementation

**Step 3.1: Create Command/Query**
```csharp
// Features/YourFeature/Commands/CreateYourFeatureCommand.cs
public class CreateYourFeatureCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
    // ... other properties
}
```

**Step 3.2: Create Handler**
```csharp
// Features/YourFeature/Commands/CreateYourFeatureCommandHandler.cs
public class CreateYourFeatureCommandHandler : IRequestHandler<CreateYourFeatureCommand, Guid>
{
    private readonly IRepository<YourEntity> _repository;
    private readonly IUnitOfWork _unitOfWork;
    
    public CreateYourFeatureCommandHandler(IRepository<YourEntity> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Guid> Handle(CreateYourFeatureCommand request, CancellationToken cancellationToken)
    {
        var entity = new YourEntity
        {
            Name = request.Name,
            // ... map properties
        };
        
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }
}
```

**Step 3.3: Create Validator (if needed)**
```csharp
// Features/YourFeature/Commands/CreateYourFeatureCommandValidator.cs
public class CreateYourFeatureCommandValidator : AbstractValidator<CreateYourFeatureCommand>
{
    public CreateYourFeatureCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("نام الزامی است")
            .MaximumLength(200).WithMessage("نام نباید بیش از 200 کاراکتر باشد");
    }
}
```

**Step 3.4: Create DTO (if needed)**
```csharp
// Common/Models/YourFeatureDto.cs
public class YourFeatureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    // ... display properties
}
```

**Step 3.5: Create Mapping**
```csharp
// Common/Mappings/MappingProfile.cs
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<YourEntity, YourFeatureDto>();
        CreateMap<CreateYourFeatureCommand, YourEntity>();
    }
}
```

#### 4. Infrastructure Layer Changes (if needed)

**If new Entity needs configuration:**
```csharp
// Data/Configurations/YourEntityConfiguration.cs
public class YourEntityConfiguration : IEntityTypeConfiguration<YourEntity>
{
    public void Configure(EntityTypeBuilder<YourEntity> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        // Relationships
        builder.HasMany(e => e.RelatedEntities)
            .WithOne(r => r.YourEntity)
            .HasForeignKey(r => r.YourEntityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

**Create Migration:**
```powershell
cd src/EazyMenu.Infrastructure
dotnet ef migrations add AddYourEntity --startup-project ../EazyMenu.Web
```

#### 5. Web Layer Implementation

**Step 5.1: Create Controller**
```csharp
// Controllers/YourFeatureController.cs or Areas/Admin/Controllers/YourFeatureController.cs
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class YourFeatureController : Controller
{
    private readonly IMediator _mediator;
    
    public YourFeatureController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    // GET: Admin/YourFeature
    public async Task<IActionResult> Index()
    {
        var items = await _mediator.Send(new GetAllYourFeaturesQuery());
        return View(items);
    }
    
    // GET: Admin/YourFeature/Create
    public IActionResult Create()
    {
        return View();
    }
    
    // POST: Admin/YourFeature/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateYourFeatureCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);
            
        var id = await _mediator.Send(command);
        TempData["Success"] = "عملیات با موفقیت انجام شد";
        return RedirectToAction(nameof(Index));
    }
}
```

**Step 5.2: Create Views**
```html
@* Views/Admin/YourFeature/Index.cshtml *@
@model IEnumerable<YourFeatureDto>

<div class="container-fluid">
    <div class="row">
        <div class="col-12">
            <h2>مدیریت YourFeature</h2>
            <a asp-action="Create" class="btn btn-primary">افزودن جدید</a>
            
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>نام</th>
                        <th>عملیات</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var item in Model)
                    {
                        <tr>
                            <td>@item.Name</td>
                            <td>
                                <a asp-action="Edit" asp-route-id="@item.Id">ویرایش</a>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>
</div>
```

#### 6. Testing
- Build the solution
- Run migrations
- Test manually
- Write unit tests (if time permits)

## 🚫 Common Pitfalls to Avoid

### ❌ Don't Do This:
1. **Breaking Clean Architecture:**
   ```csharp
   // ❌ WRONG - Domain referencing Infrastructure
   public class Restaurant : BaseEntity
   {
       [Required] // This is from Data Annotations - NO!
       public string Name { get; set; }
   }
   ```

2. **Direct DbContext in Controllers:**
   ```csharp
   // ❌ WRONG
   public class RestaurantController : Controller
   {
       private readonly ApplicationDbContext _context;
       
       public async Task<IActionResult> Index()
       {
           var restaurants = await _context.Restaurants.ToListAsync();
           return View(restaurants);
       }
   }
   ```

3. **Mixing Languages:**
   ```csharp
   // ❌ WRONG
   public async Task<IActionResult> CreateRestoran() // Mixed language
   {
       // ...
   }
   ```

4. **Forgetting Async/Await:**
   ```csharp
   // ❌ WRONG
   public IActionResult Index()
   {
       var restaurants = _mediator.Send(new GetAllRestaurantsQuery()).Result; // Blocking!
       return View(restaurants);
   }
   ```

### ✅ Do This Instead:
1. **Clean Architecture:**
   ```csharp
   // ✅ CORRECT - Domain is clean
   public class Restaurant : BaseEntity
   {
       public string Name { get; set; } = string.Empty;
   }
   
   // ✅ Validation in Application layer
   public class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
   {
       public CreateRestaurantCommandValidator()
       {
           RuleFor(x => x.Name).NotEmpty();
       }
   }
   ```

2. **Use MediatR:**
   ```csharp
   // ✅ CORRECT
   public class RestaurantController : Controller
   {
       private readonly IMediator _mediator;
       
       public async Task<IActionResult> Index()
       {
           var restaurants = await _mediator.Send(new GetAllRestaurantsQuery());
           return View(restaurants);
       }
   }
   ```

3. **Consistent Language:**
   ```csharp
   // ✅ CORRECT - English code, Persian comments
   /// <summary>
   /// ایجاد رستوران جدید
   /// </summary>
   public async Task<IActionResult> CreateRestaurant()
   {
       // ...
   }
   ```

4. **Proper Async:**
   ```csharp
   // ✅ CORRECT
   public async Task<IActionResult> Index()
   {
       var restaurants = await _mediator.Send(new GetAllRestaurantsQuery());
       return View(restaurants);
   }
   ```

## 📊 Database Migrations

### When to Create Migration
- After adding/modifying entities in Domain
- After changing entity configurations in Infrastructure
- Before testing new features

### Commands
```powershell
# Navigate to Infrastructure project
cd src/EazyMenu.Infrastructure

# Add migration
dotnet ef migrations add MigrationName --startup-project ../EazyMenu.Web

# Apply migration
dotnet ef database update --startup-project ../EazyMenu.Web

# Remove last migration (if not applied)
dotnet ef migrations remove --startup-project ../EazyMenu.Web

# List migrations
dotnet ef migrations list --startup-project ../EazyMenu.Web
```

## 🔐 Security Checklist

When implementing features:
- [ ] Use `[Authorize]` attribute on protected actions
- [ ] Validate all user inputs (FluentValidation)
- [ ] Use `[ValidateAntiForgeryToken]` on POST actions
- [ ] Sanitize HTML inputs (prevent XSS)
- [ ] Use parameterized queries (EF Core does this automatically)
- [ ] Log security events (login attempts, authorization failures)
- [ ] Never store sensitive data in plain text
- [ ] Use HTTPS only (enforced in Program.cs)

## 📝 Code Review Checklist

Before completing a feature:
- [ ] Follows Clean Architecture (no circular dependencies)
- [ ] Uses async/await properly
- [ ] Has proper error handling
- [ ] Includes Persian comments for business logic
- [ ] Follows naming conventions
- [ ] Has data validation (FluentValidation)
- [ ] Uses CancellationToken in async methods
- [ ] Views use proper layout (_Layout or _AdminLayout)
- [ ] No inline styles or scripts
- [ ] Mobile-responsive design
- [ ] RTL support for Persian content
- [ ] Builds without errors
- [ ] Migration created (if database changes)

## 🎨 UI/UX Standards

### Layout Structure
```
_Layout.cshtml (Public)
├── Header (Logo, Navigation)
├── Main Content (@RenderBody())
└── Footer

_AdminLayout.cshtml (Admin Panel)
├── Sidebar (Menu)
├── Top Bar (User info, logout)
└── Content Area (@RenderBody())

_RestaurantLayout.cshtml (Restaurant Owner)
├── Sidebar (Restaurant menu)
├── Top Bar (Restaurant selector, notifications)
└── Content Area (@RenderBody())
```

### Form Standards
```html
<form asp-action="Create" method="post">
    <div asp-validation-summary="All" class="text-danger"></div>
    
    <div class="mb-3">
        <label asp-for="Name" class="form-label">نام</label>
        <input asp-for="Name" class="form-control" />
        <span asp-validation-for="Name" class="text-danger"></span>
    </div>
    
    <button type="submit" class="btn btn-primary">ذخیره</button>
    <a asp-action="Index" class="btn btn-secondary">انصراف</a>
</form>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

## 🔄 Git Workflow

### Commit Messages (English)
```
feat: Add restaurant CRUD operations
fix: Fix order calculation bug
refactor: Improve repository pattern implementation
docs: Update AGENTS.md with new guidelines
style: Format code according to standards
test: Add unit tests for order service
```

### Branch Strategy (if using)
- `main` - Production-ready code
- `develop` - Development branch
- `feature/feature-name` - New features
- `fix/bug-name` - Bug fixes

## 📚 Key Resources

### Documentation
- `NEXT_STEPS.md` - Roadmap and next tasks
- `README.md` - Setup instructions
- `Docs/PRD.MD` - Product requirements
- `Docs/UserStories/` - Feature specifications
- `.github/copilot-instructions.md` - Coding standards

### External APIs
- **Zarinpal Docs:** https://docs.zarinpal.com/
- **Kavenegar Docs:** https://kavenegar.com/rest.html
- **EF Core:** https://learn.microsoft.com/en-us/ef/core/
- **MediatR:** https://github.com/jbogard/MediatR

## 🚀 Quick Start for Agent

When you start working:
1. Read `NEXT_STEPS.md` to see current priorities
2. Check `Docs/UserStories/` for the feature you're implementing
3. Follow the **Step-by-Step Workflow** above
4. Review the **Code Review Checklist** before finishing
5. Update `NEXT_STEPS.md` when completing major tasks

## 💡 Agent Tips

- **Always ask for clarification** if requirements are unclear
- **Suggest improvements** when you see better approaches
- **Think about edge cases** (null values, empty lists, errors)
- **Consider performance** (use pagination, avoid N+1 queries)
- **Keep user experience in mind** (loading states, error messages)
- **Write maintainable code** (future developers will thank you)

---

## ✅ بعد از هر Task موفق - الزامی!

پس از اتمام موفقیت‌آمیز هر Task، **این مراحل را حتماً انجام دهید:**

### 1️⃣ بیلد کامل پروژه
```powershell
cd "d:\Git\EazyMeny-Main"
dotnet build EazyMenu.sln
```
✅ مطمئن شوید Build بدون Error تمام شد

### 2️⃣ به‌روزرسانی مستندات

#### فایل `Docs/ProgressLog.md`
لاگ پیشرفت را ثبت کنید:
```markdown
## [YYYY-MM-DD HH:mm] - [Feature/Task Name]

### ✅ Completed:
- [توضیح دقیق کار انجام شده]
- Files: [لیست فایل‌های ایجاد/تغییر یافته]
- Changes: [خلاصه تغییرات]

### 📊 Result:
- Build: ✅ Success / ❌ Failed
- Migration: ✅ Applied / ⏸️ Pending / ➖ N/A
- Tests: [تعداد تست موفق/ناموفق]

### 🔍 Notes:
- [نکات مهم، مشکلات حل شده، یا Issues باقی‌مانده]
```

#### فایل `Docs/Todo.md`
به‌روزرسانی TODO List:
- ✅ Task تکمیل شده را check کنید
- ➕ Task های جدید کشف شده را اضافه کنید  
- 🔄 اولویت‌ها را در صورت نیاز تغییر دهید
- 📝 Dependencies بین Task ها را مشخص کنید

### 3️⃣ خلاصه نتیجه برای کاربر

به کاربر گزارش کوتاه بدهید:
```
✅ [Task Name] با موفقیت تکمیل شد!

📦 Changes:
- [فایل 1]
- [فایل 2]

🔨 Build: Success
📝 Docs: Updated

▶️ Next: [پیشنهاد Task بعدی]
```

### 4️⃣ Commit (اگر کاربر خواست)
```powershell
git add .
git commit -m "feat: [short description in English]"
```

---

**⚠️ مهم:** اگر Build یا Migration با خطا مواجه شد، **قبل از به‌روزرسانی Docs** مشکل را حل کنید!

---

**Last Updated:** October 2, 2025
**Agent Version:** 1.0
**Project Phase:** Foundation Complete - Ready for Feature Development

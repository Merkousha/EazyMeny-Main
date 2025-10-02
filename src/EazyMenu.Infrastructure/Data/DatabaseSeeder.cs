using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Domain.Entities;
using EazyMenu.Domain.Enums;
using EazyMenu.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EazyMenu.Infrastructure.Data;

/// <summary>
/// کلاس Seed کننده داده‌های اولیه برای تست
/// </summary>
public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // اطمینان از ایجاد دیتابیس
        await context.Database.MigrateAsync();

        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Users
        var (adminUser, ownerUser, customerUser) = await SeedUsersAsync(userManager);

        // Seed SubscriptionPlans (باید قبل از Subscriptions باشد)
        var plans = await SeedSubscriptionPlansAsync(context);

        // Seed Restaurants
        var restaurants = await SeedRestaurantsAsync(context, ownerUser.Id);

        // Generate QR Codes for restaurants
        var qrCodeService = serviceProvider.GetRequiredService<IQRCodeService>();
        await GenerateQRCodesAsync(restaurants, qrCodeService);

        // Seed Categories
        var categories = await SeedCategoriesAsync(context, restaurants);

        // Seed Products
        await SeedProductsAsync(context, categories);

        // Seed Subscriptions
        await SeedSubscriptionsAsync(context, ownerUser.Id, plans);

        await context.SaveChangesAsync();

        Console.WriteLine("✅ Database seeded successfully!");
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roles = { "Admin", "RestaurantOwner", "Customer" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                Console.WriteLine($"✅ Role created: {roleName}");
            }
        }
    }

    private static async Task<(ApplicationIdentityUser admin, ApplicationIdentityUser owner, ApplicationIdentityUser customer)> 
        SeedUsersAsync(UserManager<ApplicationIdentityUser> userManager)
    {
        // Admin User
        var adminEmail = "admin@eazymenu.ir";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationIdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumber = "09121234567",
                PhoneNumberConfirmed = true,
                FullName = "مدیر سیستم",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"✅ Admin user created: {adminEmail}");
            }
        }

        // Restaurant Owner User
        var ownerEmail = "owner@restaurant.com";
        var ownerUser = await userManager.FindByEmailAsync(ownerEmail);
        if (ownerUser == null)
        {
            ownerUser = new ApplicationIdentityUser
            {
                UserName = ownerEmail,
                Email = ownerEmail,
                EmailConfirmed = true,
                PhoneNumber = "09121234568",
                PhoneNumberConfirmed = true,
                FullName = "علی احمدی",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(ownerUser, "Owner@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(ownerUser, "RestaurantOwner");
                Console.WriteLine($"✅ Owner user created: {ownerEmail}");
            }
        }

        // Customer User
        var customerEmail = "customer@test.com";
        var customerUser = await userManager.FindByEmailAsync(customerEmail);
        if (customerUser == null)
        {
            customerUser = new ApplicationIdentityUser
            {
                UserName = customerEmail,
                Email = customerEmail,
                EmailConfirmed = true,
                PhoneNumber = "09121234569",
                PhoneNumberConfirmed = true,
                FullName = "محمد رضایی",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await userManager.CreateAsync(customerUser, "Customer@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(customerUser, "Customer");
                Console.WriteLine($"✅ Customer user created: {customerEmail}");
            }
        }

        return (adminUser!, ownerUser!, customerUser!);
    }

    private static async Task<List<Restaurant>> SeedRestaurantsAsync(ApplicationDbContext context, Guid ownerId)
    {
        // اگر رستوران‌ها وجود دارند، آنها را بازگردان (بدون seed جدید)
        if (await context.Restaurants.AnyAsync())
        {
            Console.WriteLine("⚠️  Restaurants already exist, skipping seed");
            return await context.Restaurants.ToListAsync();
        }

        var restaurants = new List<Restaurant>
        {
            new Restaurant
            {
                Id = Guid.NewGuid(),
                Name = "رستوران زیتون",
                NameEn = "Zeitoon Restaurant",
                Slug = "zeitoon",
                Description = "رستوران زیتون با بیش از 20 سال سابقه، ارائه دهنده بهترین غذاهای ایرانی و بین‌المللی",
                OwnerId = ownerId,
                ManagerName = "علی احمدی",
                PhoneNumber = "02188776655",
                Email = "info@zeitoon.com",
                Address = "تهران، خیابان ولیعصر، نرسیده به میدان ونک، پلاک 1234",
                WorkingHours = "{\"saturday\":\"10:00-23:00\",\"sunday\":\"10:00-23:00\",\"monday\":\"10:00-23:00\",\"tuesday\":\"10:00-23:00\",\"wednesday\":\"10:00-23:00\",\"thursday\":\"10:00-23:00\",\"friday\":\"12:00-23:00\"}",
                IsActive = true,
                AcceptOnlineOrders = true,
                AcceptReservations = true,
                DeliveryFee = 25000,
                MinimumOrderAmount = 50000,
                QRCodeUrl = "/qrcodes/zeitoon/qrcode.png",
                CreatedAt = DateTime.UtcNow
            },
            new Restaurant
            {
                Id = Guid.NewGuid(),
                Name = "فست‌فود برگر استار",
                NameEn = "Burger Star",
                Slug = "burger-star",
                Description = "بهترین برگرها و فست‌فودهای تهران با کیفیت برتر",
                OwnerId = ownerId,
                ManagerName = "رضا محمدی",
                PhoneNumber = "02188776656",
                Email = "info@burgerstar.com",
                Address = "تهران، خیابان سعادت‌آباد، مجتمع تجاری پالادیوم، طبقه 2",
                WorkingHours = "{\"saturday\":\"12:00-02:00\",\"sunday\":\"12:00-02:00\",\"monday\":\"12:00-02:00\",\"tuesday\":\"12:00-02:00\",\"wednesday\":\"12:00-02:00\",\"thursday\":\"12:00-02:00\",\"friday\":\"12:00-02:00\"}",
                IsActive = true,
                AcceptOnlineOrders = true,
                AcceptReservations = false,
                DeliveryFee = 20000,
                MinimumOrderAmount = 40000,
                QRCodeUrl = "/qrcodes/burger-star/qrcode.png",
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Restaurant
            {
                Id = Guid.NewGuid(),
                Name = "کافه‌رستوران نیلوفر",
                NameEn = "Niloofar Cafe",
                Slug = "niloofar-cafe",
                Description = "کافه‌رستوران نیلوفر با فضایی دنج و آرام، مناسب برای دورهمی‌های خانوادگی",
                OwnerId = ownerId,
                ManagerName = "سارا کریمی",
                PhoneNumber = "02188776657",
                Email = "info@niloofarcafe.com",
                Address = "تهران، خیابان شریعتی، بالاتر از پل صدر، پلاک 456",
                WorkingHours = "{\"saturday\":\"09:00-23:00\",\"sunday\":\"09:00-23:00\",\"monday\":\"09:00-23:00\",\"tuesday\":\"09:00-23:00\",\"wednesday\":\"09:00-23:00\",\"thursday\":\"09:00-23:00\",\"friday\":\"09:00-23:00\"}",
                IsActive = true,
                AcceptOnlineOrders = true,
                AcceptReservations = true,
                DeliveryFee = 30000,
                MinimumOrderAmount = 60000,
                QRCodeUrl = "/qrcodes/niloofar-cafe/qrcode.png",
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        await context.Restaurants.AddRangeAsync(restaurants);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ {restaurants.Count} restaurants seeded");

        return restaurants;
    }

    private static async Task<List<Category>> SeedCategoriesAsync(ApplicationDbContext context, List<Restaurant> restaurants)
    {
        if (await context.Categories.AnyAsync())
        {
            return await context.Categories.ToListAsync();
        }

        var categories = new List<Category>();

        // Categories for Zeitoon Restaurant
        var zeitoon = restaurants[0];
        categories.AddRange(new[]
        {
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "غذاهای ایرانی",
                NameEn = "Persian Food",
                RestaurantId = zeitoon.Id,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "پیش‌غذا",
                NameEn = "Appetizers",
                RestaurantId = zeitoon.Id,
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "نوشیدنی",
                NameEn = "Drinks",
                RestaurantId = zeitoon.Id,
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        });

        // Categories for Burger Star
        var burgerStar = restaurants[1];
        categories.AddRange(new[]
        {
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "برگر",
                NameEn = "Burgers",
                RestaurantId = burgerStar.Id,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "پیتزا",
                NameEn = "Pizza",
                RestaurantId = burgerStar.Id,
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "سیب‌زمینی",
                NameEn = "French Fries",
                RestaurantId = burgerStar.Id,
                DisplayOrder = 3,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        });

        // Categories for Niloofar Cafe
        var niloofar = restaurants[2];
        categories.AddRange(new[]
        {
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "کافی‌شاپی",
                NameEn = "Coffee Shop",
                RestaurantId = niloofar.Id,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = Guid.NewGuid(),
                Name = "دسر",
                NameEn = "Desserts",
                RestaurantId = niloofar.Id,
                DisplayOrder = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        });

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ {categories.Count} categories seeded");

        return categories;
    }

    private static async Task SeedProductsAsync(ApplicationDbContext context, List<Category> categories)
    {
        if (await context.Products.AnyAsync())
        {
            return;
        }

        var products = new List<Product>();

        // Products for Persian Food category (Zeitoon)
        var persianCategory = categories.First(c => c.Name == "غذاهای ایرانی");
        products.AddRange(new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "چلوکباب کوبیده",
                NameEn = "Chelo Kebab Koobideh",
                Description = "دو سیخ کباب کوبیده با برنج ایرانی",
                CategoryId = persianCategory.Id,
                RestaurantId = persianCategory.RestaurantId,
                Price = 180000,
                Image1Url = "/images/products/kebab-koobideh.jpg",
                IsAvailable = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "چلوکباب برگ",
                NameEn = "Chelo Kebab Barg",
                Description = "کباب برگ با برنج ایرانی و گوجه کبابی",
                CategoryId = persianCategory.Id,
                RestaurantId = persianCategory.RestaurantId,
                Price = 250000,
                Image1Url = "/images/products/kebab-barg.jpg",
                IsAvailable = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "قورمه سبزی",
                NameEn = "Ghormeh Sabzi",
                Description = "خورش قورمه سبزی با گوشت و لوبیا قرمز",
                CategoryId = persianCategory.Id,
                RestaurantId = persianCategory.RestaurantId,
                Price = 150000,
                Image1Url = "/images/products/ghormeh-sabzi.jpg",
                IsAvailable = true,
                DisplayOrder = 3,
                CreatedAt = DateTime.UtcNow
            }
        });

        // Products for Burgers category
        var burgerCategory = categories.First(c => c.Name == "برگر");
        products.AddRange(new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "برگر مخصوص",
                NameEn = "Special Burger",
                Description = "برگر دابل با پنیر، گوشت و سس مخصوص",
                CategoryId = burgerCategory.Id,
                RestaurantId = burgerCategory.RestaurantId,
                Price = 120000,
                Image1Url = "/images/products/special-burger.jpg",
                IsAvailable = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "چیزبرگر",
                NameEn = "Cheeseburger",
                Description = "برگر با پنیر چدار و سس",
                CategoryId = burgerCategory.Id,
                RestaurantId = burgerCategory.RestaurantId,
                Price = 90000,
                Image1Url = "/images/products/cheeseburger.jpg",
                IsAvailable = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            }
        });

        // Products for Coffee Shop category
        var coffeeCategory = categories.First(c => c.Name == "کافی‌شاپی");
        products.AddRange(new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "کاپوچینو",
                NameEn = "Cappuccino",
                Description = "کاپوچینو ایتالیایی با شیر تازه",
                CategoryId = coffeeCategory.Id,
                RestaurantId = coffeeCategory.RestaurantId,
                Price = 45000,
                Image1Url = "/images/products/cappuccino.jpg",
                IsAvailable = true,
                DisplayOrder = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "لاته",
                NameEn = "Latte",
                Description = "لاته با شیر فوم شده",
                CategoryId = coffeeCategory.Id,
                RestaurantId = coffeeCategory.RestaurantId,
                Price = 40000,
                Image1Url = "/images/products/latte.jpg",
                IsAvailable = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            }
        });

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ {products.Count} products seeded");
    }

    private static async Task<List<SubscriptionPlan>> SeedSubscriptionPlansAsync(ApplicationDbContext context)
    {
        // بررسی اگر پلن‌ها وجود دارند، فقط بازگردانی
        var existingPlans = await context.SubscriptionPlans.ToListAsync();
        if (existingPlans.Any())
        {
            Console.WriteLine($"⚠️  SubscriptionPlans already exist ({existingPlans.Count} plans), skipping seed");
            return existingPlans.OrderBy(p => p.PlanType).ToList();
        }
        
        Console.WriteLine("🌱 Seeding SubscriptionPlans...");

        var plans = new List<SubscriptionPlan>
        {
            // پلن پایه
            new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                PlanType = PlanType.Basic,
                Name = "پایه",
                Description = "مناسب برای رستوران‌های کوچک و استارتاپ‌ها",
                PriceMonthly = 500000, // 500 هزار تومان
                PriceYearly = 5000000, // 5 میلیون تومان (تخفیف 17%)
                MaxProducts = 50,
                MaxCategories = 10,
                MaxOrders = 100,
                HasQRCode = true,
                HasWebsite = false,
                HasReservation = false,
                HasAnalytics = false,
                SupportLevel = "ایمیل",
                Features = @"[""تا 50 محصول"",""تا 10 دسته‌بندی"",""100 سفارش در ماه"",""QR Code اختصاصی"",""پشتیبانی ایمیل""]",
                DisplayOrder = 1,
                IsActive = true,
                IsPopular = false,
                CreatedAt = DateTime.UtcNow
            },
            
            // پلن استاندارد (محبوب)
            new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                PlanType = PlanType.Standard,
                Name = "استاندارد",
                Description = "مناسب برای رستوران‌های متوسط با نیاز به امکانات بیشتر",
                PriceMonthly = 1000000, // 1 میلیون تومان
                PriceYearly = 10000000, // 10 میلیون تومان (تخفیف 17%)
                MaxProducts = 200,
                MaxCategories = 50,
                MaxOrders = 500,
                HasQRCode = true,
                HasWebsite = true,
                HasReservation = true,
                HasAnalytics = true,
                SupportLevel = "تلفنی",
                Features = @"[""تا 200 محصول"",""تا 50 دسته‌بندی"",""500 سفارش در ماه"",""QR Code اختصاصی"",""وب‌سایت اختصاصی"",""سیستم رزرو میز"",""گزارش‌گیری پایه"",""پشتیبانی تلفنی""]",
                DisplayOrder = 2,
                IsActive = true,
                IsPopular = true, // این پلن محبوب است
                CreatedAt = DateTime.UtcNow
            },
            
            // پلن پیشرفته
            new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                PlanType = PlanType.Premium,
                Name = "پیشرفته",
                Description = "مناسب برای رستوران‌های بزرگ و زنجیره‌ای",
                PriceMonthly = 2000000, // 2 میلیون تومان
                PriceYearly = 20000000, // 20 میلیون تومان (تخفیف 17%)
                MaxProducts = -1, // نامحدود
                MaxCategories = -1, // نامحدود
                MaxOrders = -1, // نامحدود
                HasQRCode = true,
                HasWebsite = true,
                HasReservation = true,
                HasAnalytics = true,
                SupportLevel = "اختصاصی 24/7",
                Features = @"[""محصولات نامحدود"",""دسته‌بندی نامحدود"",""سفارش نامحدود"",""QR Code اختصاصی"",""وب‌سایت اختصاصی با قالب‌های متعدد"",""سیستم رزرو پیشرفته"",""گزارش‌گیری تحلیلی"",""مدیریت چند شعبه"",""پشتیبانی اختصاصی 24/7""]",
                DisplayOrder = 3,
                IsActive = true,
                IsPopular = false,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.SubscriptionPlans.AddRangeAsync(plans);
        await context.SaveChangesAsync();
        
        Console.WriteLine($"✅ {plans.Count} SubscriptionPlans seeded successfully");
        
        return plans;
    }

    private static async Task SeedSubscriptionsAsync(ApplicationDbContext context, Guid userId, List<SubscriptionPlan> plans)
    {
        if (await context.Subscriptions.AnyAsync())
        {
            return;
        }

        // Get first restaurant for the owner
        var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.OwnerId == userId);
        if (restaurant == null)
        {
            Console.WriteLine("⚠️  No restaurant found for subscription seeding");
            return;
        }

        // Get Standard plan (محبوب‌ترین پلن)
        var standardPlan = plans.FirstOrDefault(p => p.PlanType == PlanType.Standard);
        if (standardPlan == null)
        {
            Console.WriteLine("⚠️  Standard plan not found for subscription seeding");
            return;
        }

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            RestaurantId = restaurant.Id,
            SubscriptionPlanId = standardPlan.Id,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow.AddDays(60), // 60 روز باقیمانده
            Amount = standardPlan.PriceMonthly * 3, // 3 ماهه
            IsYearly = false,
            AutoRenew = true,
            MaxProducts = standardPlan.MaxProducts,
            MaxOrdersPerMonth = standardPlan.MaxOrders,
            HasReservationFeature = standardPlan.HasReservation,
            HasWebsiteBuilder = standardPlan.HasWebsite,
            HasAdvancedReporting = standardPlan.HasAnalytics,
            CurrentProductCount = 0,
            CurrentMonthOrderCount = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };

        await context.Subscriptions.AddAsync(subscription);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Subscription seeded for restaurant: {restaurant.Name} (Plan: {standardPlan.Name})");
    }

    private static async Task GenerateQRCodesAsync(List<Restaurant> restaurants, IQRCodeService qrCodeService)
    {
        foreach (var restaurant in restaurants)
        {
            try
            {
                // بررسی اگر QR Code قبلاً ساخته شده و فایل وجود دارد
                if (!string.IsNullOrEmpty(restaurant.QRCodeUrl))
                {
                    // مسیر کامل فایل را بررسی می‌کنیم
                    var wwwrootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    var fullPath = Path.Combine(wwwrootPath, restaurant.QRCodeUrl.TrimStart('/'));
                    
                    if (File.Exists(fullPath))
                    {
                        Console.WriteLine($"⏭️  QR Code already exists for {restaurant.Name}: {restaurant.QRCodeUrl}");
                        continue; // QR موجود است، رد شو
                    }
                    else
                    {
                        Console.WriteLine($"⚠️  QR Code path exists but file missing for {restaurant.Name}, regenerating...");
                    }
                }
                
                // ساخت QR Code جدید
                var qrCodeUrl = $"https://eazymenu.ir/menu/{restaurant.Slug}";
                var savedPath = await qrCodeService.SaveQRCodeAsync(restaurant.Slug, qrCodeUrl, 300);
                
                // Update restaurant QRCodeUrl with the actual saved path
                restaurant.QRCodeUrl = savedPath;
                
                Console.WriteLine($"✅ QR Code generated for {restaurant.Name}: {savedPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed to generate QR Code for {restaurant.Name}: {ex.Message}");
            }
        }
    }
}

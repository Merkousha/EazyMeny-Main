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

        // Seed Restaurants
        var restaurants = await SeedRestaurantsAsync(context, ownerUser.Id);

        // Seed Categories
        var categories = await SeedCategoriesAsync(context, restaurants);

        // Seed Products
        await SeedProductsAsync(context, categories);

        // Seed Subscriptions
        await SeedSubscriptionsAsync(context, ownerUser.Id);

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
                FirstName = "مدیر",
                LastName = "سیستم",
                IsActive = true
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
                FirstName = "علی",
                LastName = "احمدی",
                IsActive = true
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
                FirstName = "محمد",
                LastName = "رضایی",
                IsActive = true
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
        if (await context.Restaurants.AnyAsync())
        {
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
                ImageUrl = "/images/products/kebab-koobideh.jpg",
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
                ImageUrl = "/images/products/kebab-barg.jpg",
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
                ImageUrl = "/images/products/ghormeh-sabzi.jpg",
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
                ImageUrl = "/images/products/special-burger.jpg",
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
                ImageUrl = "/images/products/cheeseburger.jpg",
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
                ImageUrl = "/images/products/cappuccino.jpg",
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
                ImageUrl = "/images/products/latte.jpg",
                IsAvailable = true,
                DisplayOrder = 2,
                CreatedAt = DateTime.UtcNow
            }
        });

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ {products.Count} products seeded");
    }

    private static async Task SeedSubscriptionsAsync(ApplicationDbContext context, Guid userId)
    {
        if (await context.Subscriptions.AnyAsync())
        {
            return;
        }

        var subscription = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Plan = SubscriptionPlan.Standard,
            Status = SubscriptionStatus.Active,
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow.AddDays(60),
            Price = 500000,
            IsTrial = false,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };

        await context.Subscriptions.AddAsync(subscription);
        await context.SaveChangesAsync();
        Console.WriteLine($"✅ Subscription seeded for user");
    }
}

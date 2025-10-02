using EazyMenu.Application.Common.Interfaces;
using EazyMenu.Application.Common.Models.Dashboard;
using EazyMenu.Domain.Entities;
using MediatR;

namespace EazyMenu.Application.Features.Dashboard.Queries.GetDashboardStats;

/// <summary>
/// Handler برای دریافت آمار داشبورد ادمین
/// </summary>
public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IRepository<Restaurant> _restaurantRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<ApplicationUser> _userRepository;

    public GetDashboardStatsQueryHandler(
        IRepository<Restaurant> restaurantRepository,
        IRepository<Category> categoryRepository,
        IRepository<Product> productRepository,
        IRepository<ApplicationUser> userRepository)
    {
        _restaurantRepository = restaurantRepository;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        // دریافت تمام داده‌ها
        var restaurants = await _restaurantRepository.GetAllAsync(cancellationToken);
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var products = await _productRepository.GetAllAsync(cancellationToken);
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var today = DateTime.Today;
        var weekAgo = today.AddDays(-7);
        var monthAgo = today.AddMonths(-1);

        var stats = new DashboardStatsDto
        {
            // آمار کلی
            TotalRestaurants = restaurants.Count(),
            ActiveRestaurants = restaurants.Count(r => r.IsActive),
            TotalCategories = categories.Count(),
            TotalProducts = products.Count(),
            ActiveProducts = products.Count(p => p.IsActive && p.IsAvailable),
            TotalUsers = users.Count(),
            
            // آمار زمانی
            TodayRestaurants = restaurants.Count(r => r.CreatedAt.Date == today),
            WeekRestaurants = restaurants.Count(r => r.CreatedAt >= weekAgo),
            MonthRestaurants = restaurants.Count(r => r.CreatedAt >= monthAgo)
        };

        return stats;
    }
}

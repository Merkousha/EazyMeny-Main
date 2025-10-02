using EazyMenu.Application.Common.Models.Dashboard;
using MediatR;

namespace EazyMenu.Application.Features.Dashboard.Queries.GetDashboardStats;

/// <summary>
/// Query برای دریافت آمار داشبورد ادمین
/// </summary>
public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
{
}

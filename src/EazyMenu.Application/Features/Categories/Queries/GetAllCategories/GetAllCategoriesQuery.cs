using EazyMenu.Application.Common.Models.Category;
using MediatR;

namespace EazyMenu.Application.Features.Categories.Queries.GetAllCategories;

/// <summary>
/// Query برای دریافت تمام دسته‌بندی‌ها (برای ادمین)
/// </summary>
public class GetAllCategoriesQuery : IRequest<List<CategoryListDto>>
{
    // می‌توان پارامترهای Filter و Pagination اضافه کرد
}

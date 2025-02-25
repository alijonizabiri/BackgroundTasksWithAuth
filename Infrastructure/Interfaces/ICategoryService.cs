using Domain.Dtos.Category;
using Domain.Filters;
using Infrastructure.Responses;

namespace Infrastructure.Interfaces;

public interface ICategoryService
{
    Task<PaginationResponse<List<GetCategoryDto>>> GetAll(CategoryFilter filter);
    Task<Response<string>> CreateCategory(AddCategoryDto request);
    Task<Response<string>> UpdateCategory(UpdateCategoryDto request);
    Task<Response<string>> DeleteCategory(int id);
}

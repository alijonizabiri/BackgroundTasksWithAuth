using System.Linq.Expressions;
using Domain.Dtos.Category;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.Responses;

namespace Infrastructure.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAll(CategoryFilter filter);
    Task<Category> GetCategory(Expression<Func<Category, bool>>? filter = null);
    Task<int> CreateCategory(Category request);
    Task<int> UpdateCategory(Category request);
    Task<int> DeleteCategory(Category request);
}

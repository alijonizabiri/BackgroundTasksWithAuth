using System.Net;
using Domain.Dtos.Category;
using Domain.Entities;
using Domain.Filters;
using Infrastructure.Interfaces;
using Infrastructure.Responses;

namespace Infrastructure.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<PaginationResponse<List<GetCategoryDto>>> GetAll(CategoryFilter filter)
    {
        var categories = await categoryRepository.GetAll(filter);

        var totalRecords = categories.Count;
        var data = categories
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();

        var result = data.Select(c => new GetCategoryDto()
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();

        return new PaginationResponse<List<GetCategoryDto>>(result, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<Response<string>> CreateCategory(AddCategoryDto request)
    {
        var category = new Category()
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await categoryRepository.CreateCategory(category);

        return result == 1
            ? new Response<string>("Success")
            : new Response<string>(HttpStatusCode.BadRequest, "Failed");
    }

    public async Task<Response<string>> UpdateCategory(UpdateCategoryDto request)
    {
        var category = await categoryRepository.GetCategory(c=>c.Id == request.Id);

        if (category == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");
        }

        category.Name = request.Name;
        category.Description = request.Description;

        var result = await categoryRepository.UpdateCategory(category);
        return result == 1
            ? new Response<string>("Success")
            : new Response<string>(HttpStatusCode.BadRequest, "Failed");
    }

    public async Task<Response<string>> DeleteCategory(int id)
    {
        var category = await categoryRepository.GetCategory(c=>c.Id == id);
        if (category == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "Category not found");
        }

        var result = await categoryRepository.DeleteCategory(category);
        return result == 1
            ? new Response<string>("Success")
            : new Response<string>(HttpStatusCode.BadRequest, "Failed");
    }
}

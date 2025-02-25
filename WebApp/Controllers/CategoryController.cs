using Domain.Dtos.Category;
using Domain.Filters;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery]CategoryFilter filter)
    {
        var categories = await categoryService.GetAll(filter);
        return StatusCode(categories.StatusCode, categories);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory(AddCategoryDto request)
    {
        var response = await categoryService.CreateCategory(request);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto request)
    {
        var response = await categoryService.UpdateCategory(request);
        return StatusCode(response.StatusCode, response);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var response = await categoryService.DeleteCategory(id);
        return StatusCode(response.StatusCode, response);
    }
}

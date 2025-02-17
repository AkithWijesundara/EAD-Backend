using EAD_Backend.Models;
using EAD_Backend.OtherModels;
using EAD_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //! ================== Category Endpoints =========================>

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategories();
            return Ok(new ApiResponse<IEnumerable<Category>>("Categories retrieved successfully", categories));
        }

        [HttpPost("categories/create")]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto category)
        {
            var createdCategory = await _categoryService.CreateCategory(category);
            return Ok(new ApiResponse<Category>("Category created successfully", createdCategory));
        }

        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            var category = await _categoryService.GetCategoryById(id);
            return Ok(new ApiResponse<Category>("Category retrieved successfully", category));
        }



        [HttpPut("categories/update/{id}")]
        public async Task<IActionResult> UpdateCategory(string id, CategoryUpdateDto category)
        {
            var updatedCategory = await _categoryService.UpdateCategory(id, category);
            return Ok(new ApiResponse<Category>("Category updated successfully", updatedCategory));
        }

        [HttpDelete("categories/delete/{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var result = await _categoryService.DeleteCategory(id);
            return Ok(new ApiResponse<bool>("Category deleted successfully", result));
        }

        //! ================== SubCategory Endpoints =========================>

        [HttpGet("subcategories")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var subCategories = await _categoryService.GetAllSubCategories();
            return Ok(new ApiResponse<IEnumerable<SubCategory>>("Subcategories retrieved successfully", subCategories));
        }

        [HttpGet("subcategories/{id}")]
        public async Task<IActionResult> GetSubCategoryById(string id)
        {
            var subCategory = await _categoryService.GetSubCategoryById(id);
            return Ok(new ApiResponse<SubCategory>("Subcategory retrieved successfully", subCategory));
        }

        [HttpPost("subcategories/create")]
        public async Task<IActionResult> CreateSubCategory(CreateSubCategoryDto subCategory)
        {
            var createdSubCategory = await _categoryService.CreateSubCategory(subCategory);
            return Ok(new ApiResponse<SubCategory>("Subcategory created successfully", createdSubCategory));
        }

        [HttpPut("subcategories/update/{id}")]
        public async Task<IActionResult> UpdateSubCategory(string id, UpdateSubCategoryDto subCategory)
        {
            var updatedSubCategory = await _categoryService.UpdateSubCategory(id, subCategory);
            return Ok(new ApiResponse<SubCategory>("Subcategory updated successfully", updatedSubCategory));
        }

        [HttpDelete("subcategories/delete/{id}")]
        public async Task<IActionResult> DeleteSubCategory(string id)
        {
            var result = await _categoryService.DeleteSubCategory(id);
            return Ok(new ApiResponse<bool>("Subcategory deleted successfully", result));
        }

        [HttpGet("subcategories/by-category/{categoryId}")]
        public async Task<IActionResult> GetSubCategoriesByCategoryId(string categoryId)
        {
            var subCategories = await _categoryService.GetSubCategoriesByCategoryId(categoryId);
            return Ok(new ApiResponse<IEnumerable<SubCategory>>("Subcategories retrieved successfully", subCategories));
        }
    }
}

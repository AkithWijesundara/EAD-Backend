using MongoDB.Driver;
using EAD_Backend.Models;
using EAD_Backend.Data;
using MongoDB.Bson;

namespace EAD_Backend.Services
{
    public class CategoryService
    {
        private readonly IMongoCollection<Category> _categoryModel;
        private readonly IMongoCollection<SubCategory> _subCategoryModel;

        public CategoryService(MongoDBService mongoDbService)
        {
            _categoryModel = mongoDbService.Database?.GetCollection<Category>("categories");
            _subCategoryModel = mongoDbService.Database?.GetCollection<SubCategory>("subcategories");
        }

        // Get all categories
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _categoryModel.Find(category => true).ToListAsync();
        }

        // Get a category by ID
        public async Task<Category> GetCategoryById(string id)
        {
            return await _categoryModel.Find(category => category.Id == id).FirstOrDefaultAsync();
        }

        // Create a new category
        // Create a new category
        public async Task<Category> CreateCategory(CategoryCreateDto categoryDto)
        {
            // Map CategoryCreateDto to Category
            var category = new Category
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = categoryDto.Name
            };

            // Insert the mapped Category into the database
            await _categoryModel.InsertOneAsync(category);

            return category;
        }



        public async Task<Category> UpdateCategory(string id, CategoryUpdateDto categoryDto)
        {
            // Fetch the existing category
            var existingCategory = await _categoryModel.Find(c => c.Id == id).FirstOrDefaultAsync();

            if (existingCategory == null)
            {
                throw new ArgumentException("Category not found");
            }

            // Map CategoryUpdateDto to Category
            existingCategory.Name = categoryDto.Name;

            // Update the category in the database
            await _categoryModel.ReplaceOneAsync(c => c.Id == id, existingCategory);

            return existingCategory;
        }


        // Delete a category by ID
        public async Task<bool> DeleteCategory(string id)
        {
            var result = await _categoryModel.DeleteOneAsync(c => c.Id == id);
            return result.DeletedCount > 0;
        }


        //! Sub Category Services

        // Get all subcategories
        public async Task<IEnumerable<SubCategory>> GetAllSubCategories()
        {
            return await _subCategoryModel.Find(subCategory => true).ToListAsync();
        }

        // Get a subcategory by ID
        public async Task<SubCategory> GetSubCategoryById(string id)
        {
            return await _subCategoryModel.Find(subCategory => subCategory.Id == id).FirstOrDefaultAsync();
        }

        // Create a new subcategory
        // Create a new subcategory
        public async Task<SubCategory> CreateSubCategory(CreateSubCategoryDto subCategoryDto)
        {
            // Map CreateSubCategoryDto to SubCategory
            var subCategory = new SubCategory
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = subCategoryDto.Name,
                CategoryId = subCategoryDto.CategoryId
            };

            // Insert the mapped SubCategory into the database
            await _subCategoryModel.InsertOneAsync(subCategory);

            return subCategory;
        }


        // Update an existing subcategory
        public async Task<SubCategory> UpdateSubCategory(string id, UpdateSubCategoryDto subCategoryDto)
        {
            // Fetch the existing subcategory from the database
            var existingSubCategory = await _subCategoryModel.Find(sc => sc.Id == id).FirstOrDefaultAsync();

            if (existingSubCategory == null)
            {
                throw new ArgumentException("Subcategory not found");
            }

            // Map the fields from UpdateSubCategoryDto to the existing SubCategory object
            existingSubCategory.Name = subCategoryDto.Name;
            existingSubCategory.CategoryId = subCategoryDto.CategoryId;

            // Update the subcategory in the database
            await _subCategoryModel.ReplaceOneAsync(sc => sc.Id == id, existingSubCategory);

            return existingSubCategory;
        }


        // Delete a subcategory by ID
        public async Task<bool> DeleteSubCategory(string id)
        {
            var result = await _subCategoryModel.DeleteOneAsync(sc => sc.Id == id);
            return result.DeletedCount > 0;
        }

        // Get subcategories by category ID
        public async Task<IEnumerable<SubCategory>> GetSubCategoriesByCategoryId(string categoryId)
        {
            return await _subCategoryModel.Find(sc => sc.CategoryId == categoryId).ToListAsync();
        }
    }
}

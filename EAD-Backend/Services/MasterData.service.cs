/*
 * File: Master Data Service
 * Author: Perera V. H. P.
 * Description: This file contains service functions for master data.
 * Created: 07/10/2024
*/


using EAD_Backend.Data;
using EAD_Backend.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EAD_Backend.Services
{

    public class MasterDataService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<Role> _roleModel;
        private readonly IMongoCollection<Category> _categoryModel;
        private readonly IMongoCollection<SubCategory> _subCategoryModel;

        public MasterDataService(MongoDBService mongoDbService, IConfiguration configuration)
        {
            _configuration = configuration;
            // _userModel = mongoDbService.Database?.GetCollection<User>("users");
            _roleModel = mongoDbService.Database?.GetCollection<Role>("roles");
            _categoryModel = mongoDbService.Database?.GetCollection<Category>("categories");
            _subCategoryModel = mongoDbService.Database?.GetCollection<SubCategory>("subcategories");

        }

        public List<Role> GetRoles()
        {
            return _roleModel.Find(role => true).ToList();
        }

        public List<Category> GetCategories()
        {
            return _categoryModel.Find(category => true).ToList();
        }

        //! get sub categories by category id
        public async Task<List<SubCategory>> GetSubCategoriesByCategoryId(string categoryId)
        {
            var filter = Builders<SubCategory>.Filter.Eq(subCategory => subCategory.CategoryId, categoryId);
            return await _subCategoryModel.Find(filter).ToListAsync();
        }

        //get sub category by id
        public async Task<SubCategory> GetSubCategoryById(string id)
        {
            return await _subCategoryModel.Find(subCategory => subCategory.Id == id).FirstOrDefaultAsync();
        }





    }
}
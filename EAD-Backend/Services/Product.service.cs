/*
 * File: Product Service
 * Author: Perera V. H. P.
 * Description: This file contains Service Functions for Product management
 * Created: 07/10/2024
*/


using EAD_Backend.Data;
using EAD_Backend.DTOs;
using EAD_Backend.Models;
using MongoDB.Driver;

namespace EAD_Backend.Services
{
    public class ProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<Product> _productModel;

        public ProductService(MongoDBService mongoDbService, IConfiguration configuration)
        {
            _configuration = configuration;
            _productModel = mongoDbService.Database?.GetCollection<Product>("products");
        }

        //! =======================================================  Define Business | DB Operations for Product ===================================>
        //! Get all products
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productModel.Find(product => true).ToListAsync();
        }

        //! create a product
        public async Task<Product> Create(string userId, Product product)
        {

            product.VendorId = userId;
            product.Active = true;
            await _productModel.InsertOneAsync(product);
            return product;
        }

        //! Update a product
        public async Task<object> Update(string id, UpdateProductDto product)
        {
            var filter = Builders<Product>.Filter.Eq("Id", id);
            var update = Builders<Product>.Update
                .Set("Name", product.Name)
                .Set("Description", product.Description)
                .Set("Price", product.Price)
                .Set("Category", product.Category)
                .Set("Images", product.Images)
                .Set("StockCount", product.StockCount);

            await _productModel.UpdateOneAsync(filter, update);
            return product;
        }

        //! Delete a product
        public async Task<Product> Delete(string id)
        {
            var filter = Builders<Product>.Filter.Eq("Id", id);
            return await _productModel.FindOneAndDeleteAsync(filter);
        }

        //! Get a product by Id
        public async Task<Product> GetById(string id)
        {
            var filter = Builders<Product>.Filter.Eq("Id", id);
            return await _productModel.Find(filter).FirstOrDefaultAsync();
        }

    }
}
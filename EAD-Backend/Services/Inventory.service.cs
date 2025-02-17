/*
File: Inventory.service.cs
Author: Rathnayaka M.R.T.N
Description: Inventory service class for Inventory management in the EAD project
created:  02/10/2024
*/

using EAD_Backend.Models;
using MongoDB.Driver;
using EAD_Backend.Data;
using EAD_Backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend.Services

{
    public class InventoryService
    {
        private readonly IConfiguration _configuration;
        private readonly IMongoCollection<Product> _productModel;
        private readonly IMongoCollection<OrderLine> _orderModel;
        private readonly MasterDataService _masterDataService;
        private readonly NotificationService _notificationService;


        // Constructor
        public InventoryService(MongoDBService mongoDbService, IConfiguration configuration, MasterDataService masterDataService, NotificationService notificationService)
        {
            _configuration = configuration;
            _productModel = mongoDbService.Database?.GetCollection<Product>("products");
            _orderModel = mongoDbService.Database?.GetCollection<OrderLine>("OrderLines");
            _masterDataService = masterDataService;
            _notificationService = notificationService;
        }

        //! =======================================================  Define Business | DB Operations for Inventory ===================================>



        // Retrieve all products to see stock levels
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _productModel.Find(product => true).ToListAsync();
        }
        // Retrieve product by ID
        public async Task<Product> GetProductByIdAsync(string id)
        {
            return await _productModel.Find(product => product.Id == id).FirstOrDefaultAsync();
        }


        // Method to retrieve all products for a specific vendor and group by sub-category
        public async Task<Dictionary<string, List<Product>>> GetProductsByVendorAndGroupBySubCategoryAsync(string vendorId)
        {

            var filter = Builders<Product>.Filter.Eq(p => p.VendorId, vendorId);
            var products = await _productModel.Find(filter).ToListAsync();

            // Populate SubCategoryName for each product
            foreach (var product in products)
            {
                var subCategory = await _masterDataService.GetSubCategoryById(product.SubCategory);
                product.SubCategoryName = subCategory?.Name;
            }


            var groupedProducts = products
                .GroupBy(p => p.SubCategoryName)
                .ToDictionary(g => g.Key, g => g.ToList());

            return groupedProducts;
        }


        // Create a new product (inventory item)
        public async Task CreateProductAsync(Product product)
        {
            await _productModel.InsertOneAsync(product);
            CheckLowStock(product);
        }

        // Update stock levels when updating a product
        public async Task UpdateProductAsync(string id, int stockCount)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
            var update = Builders<Product>.Update
                .Set(p => p.StockCount, stockCount);
            await _productModel.UpdateOneAsync(filter, update);
            var updatedProductFromDb = await GetProductByIdAsync(id);
            CheckLowStock(updatedProductFromDb);
        }

        // Delete a product only if it is not part of a pending order by checking ordermodel status
        public async Task<bool> DeleteProductAsync(string id)
        {
            var filter = Builders<OrderLine>.Filter.Eq(o => o.ProductNo, id);
            var orders = await _orderModel.Find(filter).ToListAsync();


            if (orders.Any(o => o.Status.Trim().Equals("Pending", StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }

            // If no pending orders, proceed to delete the product
            var deleteResult = await _productModel.DeleteOneAsync(p => p.Id == id);
            return (deleteResult.DeletedCount > 0);
        }




        // Check if stock is low and notify vendor
        private void CheckLowStock(Product product)
        {
            if (product.StockCount <= product.LowStockThreshold)

                NotifyVendor(product.VendorId, product.Name, product.StockCount, product.Id);
        }


        // Notify vendor when stock is low
        public void NotifyVendor(string vendorId, string productName, int stockCount, string productId)
        {
            var notification = new Notification
            {
                Title = "Low Stock Alert",
                Message = $"Stock for {productName} is low.\n" +
                  $"Current stock count: {stockCount}\n",
                UserId = vendorId
            };
            // Call the notification service to send the notification
            _notificationService.CreateNotificationAsync(notification);
        }
    }
}




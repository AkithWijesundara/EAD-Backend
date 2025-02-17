/*
 * File: Feedback Service
 * Author:Udumulla C.J
 * Description: This file contains Service Functions for Feedback management
 * Created: 08/10/2024
*/


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EAD_Backend.Data;
using EAD_Backend.DTOs;
using EAD_Backend.Models;
using MongoDB.Driver;

namespace EAD_Backend.Services
{
    public class FeedbackService
    {
        private readonly IMongoCollection<Feedback> _feedbackModel;
        private readonly IMongoCollection<Product> _productModel;

               private readonly IMongoCollection<User> _userModel;

        public FeedbackService(MongoDBService mongoDbService)
        {
            _feedbackModel = mongoDbService.Database?.GetCollection<Feedback>("feedbacks");
            _productModel = mongoDbService.Database?.GetCollection<Product>("products");
            _userModel = mongoDbService.Database?.GetCollection<User>("users");
        }

        //! =======================================================  Define Business | DB Operations for Feedback ===================================>
        //! Get all feedbacks
        public async Task<IEnumerable<Feedback>> GetAll()
        {
            return await _feedbackModel.Find(feedback => true).ToListAsync();
        }

        //! create a feedback
        public async Task<Feedback> Create(Feedback feedback)
        {
            await _feedbackModel.InsertOneAsync(feedback);
            return feedback;
        }

        //! update a feedback
        public async Task<Feedback> Update(string id, Feedback feedback)
        {
            
            var filter = Builders<Feedback>.Filter.Eq("Id", id);
            var update = Builders<Feedback>.Update
                .Set("Rating", feedback.Rating)
                .Set("Message", feedback.Message);
            await _feedbackModel.UpdateOneAsync(filter, update);
            return feedback;
        }

        public async Task<Feedback>GetFeedbackById(string id)
        {
            return await _feedbackModel.Find(feedback => feedback.Id == id).FirstOrDefaultAsync();
        }


        /* get feetback for the customer */
        public async Task<IEnumerable<Feedback>> GetFeedbackForCustomer(string customerId)
        {
       
            return await _feedbackModel.Find(feedback => feedback.CustomerId == customerId).ToListAsync();

        }

        /*get feetback for the customer for a specific product */
        public async Task<IEnumerable<Feedback>> GetFeedbackForCustomerForProduct(string customerId, string productId)
        {
            return await _feedbackModel.Find(feedback => feedback.CustomerId == customerId && feedback.ProductId == productId).ToListAsync();
        }

        /* get feedback for the product */
        public async Task<IEnumerable<Feedback>> GetFeedbackForProduct(string productId)
        {
            return await _feedbackModel.Find(feedback => feedback.ProductId == productId).ToListAsync();

        }

        /* get average rating for a specific product */
        public async Task<double> GetAverageRatingForProduct(string productId)
        {
            var feedbacks = await _feedbackModel.Find(feedback => feedback.ProductId == productId).ToListAsync();
            if (feedbacks.Count() == 0)
            {
                return 0;
            }
            double sum = 0;
            foreach (var feedback in feedbacks)
            {
                sum += feedback.Rating ?? 0;
            }
            return sum / feedbacks.Count();

        }

        /* get average rating for a vendor vendor ID is in product db as vendorId */
        public async Task<double> GetAverageRatingForVendor(string vendorId)
        {
            var products = await _productModel.Find(product => product.VendorId == vendorId).ToListAsync();
            if (products.Count == 0)
            {
            return 0;
            }

            double sum = 0;
            int count = 0;

            foreach (var product in products)
            {
            var feedbacks = await _feedbackModel.Find(feedback => feedback.ProductId == product.Id).ToListAsync();
            foreach (var feedback in feedbacks)
            {
                sum += feedback.Rating ?? 0;
                count++;
            }
            }

            if (count == 0)
            {
            return 0;
            }

            return sum / count;
        }

    
      // Method to get feedback for all products associated with a vendor and calculate average rating
    public async Task<IEnumerable<object>> GetFeedbackForVendor(string vendorId)
    {
         
        // Step 1: Get all products for the given vendor
        var products = await _productModel.Find(product => product.VendorId == vendorId).ToListAsync();

        // If no products are found, return an empty list
        if (products.Count == 0)
        {
            return new List<object>();
        }

        // Step 2: Get product IDs from the vendor's products
        var productIds = products.Select(p => p.Id).ToList();

        // Step 3: Query the feedbacks for all products of the vendor
        var feedbacks = await _feedbackModel.Find(feedback => productIds.Contains(feedback.ProductId)).ToListAsync();

        // Extract customerIds from feedbacks
        var customerIds = feedbacks.Select(f => f.CustomerId).Distinct().ToList();
        var customers = await _userModel.Find(customer => customerIds.Contains(customer.Id)).ToListAsync();

        // Step 4: Combine the product and feedback information, calculate average rating
        var productFeedbacks = products.Select(product =>
        {
            var productFeedbacks = feedbacks.Where(f => f.ProductId == product.Id).ToList();

  // Embed customer details within each feedback
        var feedbacksWithCustomerDetails = productFeedbacks.Select(feedback => new
        {
            Id = feedback.Id,
            CustomerId = customers.FirstOrDefault(customer => customer.Id == feedback.CustomerId),
            ProductId = feedback.ProductId,
            Message = feedback.Message,
            Rating = feedback.Rating
            }).ToList();
           
           
            
            // Calculate average rating for this product
            double averageRating = 0;
            if (productFeedbacks.Count > 0)
            {
                averageRating = productFeedbacks.Average(f => (double)(f.Rating ?? 0)); // Ensure null ratings default to 0
            }

            return new
            {
                Product = product,
                Feedbacks = feedbacksWithCustomerDetails,
                AverageRating = averageRating, // Add the average rating for each product
                
            };
        }).ToList();

        return productFeedbacks;
    }


   
    }
    

    }
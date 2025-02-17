/*
File: Feedback.controller.cs
Author: Udumulla C.J.
Description: Feedback controller class for the EAD project
created:  02/10/2024
*/

using EAD_Backend.DTOs;
using EAD_Backend.Models;
using EAD_Backend.OtherModels;
using EAD_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        // Constructor injection
        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        //! ======================================================== Define API Endpoints ============================================================>

        //! Get all feedbacks
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _feedbackService.GetAll();
            return Ok(new ApiResponse<object>("Successful", response));
        }

        //! Create a feedback
        [Authorize(Roles = "Customer")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(Feedback feedback)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>("Failed", "User not found"));
            }

            feedback.CustomerId = userId;


            var response = await _feedbackService.Create(feedback);
            return Ok(new ApiResponse<Feedback>("Create successful", response));
        }

        //! Update a feedback
        [Authorize(Roles = "Customer")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, Feedback feedback)
        {
            var userId = User.FindFirst("UserId")?.Value;

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>("Failed", "User not found"));
            }

            var feedbackInDb = await _feedbackService.GetFeedbackById(id);
            if (feedbackInDb == null)
            {
                return BadRequest(new ApiResponse<object>("Failed", "Feedback not found"));
            }

            if (feedbackInDb.CustomerId != userId)
            {
                return BadRequest(new ApiResponse<object>("Failed", "You are not authorized to update this feedback"));
            }
            var response = await _feedbackService.Update(id, feedback);
            return Ok(new ApiResponse<object>("Update successful", response));
        }

        //!Get feedback by Id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(string id)
        {
            var response = await _feedbackService.GetFeedbackById(id);
            return Ok(new ApiResponse<object>("Successful", response));
        }
        //!Get feedback for the product
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetFeedbackForProduct(string productId)
        {
            var response = await _feedbackService.GetFeedbackForProduct(productId);
            return Ok(new ApiResponse<object>("Successful", response));
        }

        //!Get feedback for the customer
        [HttpGet("customer")]
        public async Task<IActionResult> GetFeedbackForCustomer()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var response = await _feedbackService.GetFeedbackForCustomer(userId);
            return Ok(new ApiResponse<object>("Successful", response));
        }

        //!Get feedback for the customer for a specific product
        [HttpGet("customer/{productId}")]
        public async Task<IActionResult> GetFeedbackForCustomer(string productId)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var response = await _feedbackService.GetFeedbackForCustomerForProduct(userId, productId);
            return Ok(new ApiResponse<object>("Successful", response));
        }

        //!Get average rating for a specific product
        [HttpGet("average/{productId}")]
        public async Task<IActionResult> GetAverageRatingForProduct(string productId)
        {
            var response = await _feedbackService.GetAverageRatingForProduct(productId);
            return Ok(new ApiResponse<object>("Successful", response));
        }

        /*!get average rating for the vendor for all of his products */
        [HttpGet("average/vendor")]
        public async Task<IActionResult> GetAverageRatingForVendor()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var response = await _feedbackService.GetAverageRatingForVendor(userId);
            return Ok(new ApiResponse<object>("Successful", response));
        }

        [Authorize(Roles = "Vendor,Admin")]
        [HttpGet("products/reviews")]
        public async Task<IActionResult> GetFeedbackForAllProducts()
        {
            // Get userId from the token
            var userId = User.FindFirst("UserId")?.Value;

            if (userId == null)
            {
                return BadRequest(new ApiResponse<object>("Failed", "User not found"));
            }

            var response = await _feedbackService.GetFeedbackForVendor(userId);

            return Ok(new ApiResponse<object>("Successful", response));
        }



    }
}

/*
File: Inventory.controller.cs
Author: Rathnayaka M.R.T.N
Description: Inventory controller class for Inventory management in the EAD project
created:  02/10/2024
*/

using EAD_Backend.Models;
using EAD_Backend.OtherModels;
using EAD_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EAD_Backend.DTOs;

namespace EAD_Backend.Controllers

{

    [Route("api/[controller]")]
    [ApiController]

    // Inventory controller class
    public class InventoryController : ControllerBase

    {
        private readonly InventoryService _inventoryService;


        // Constructor
        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }


        //! ======================================================== Define API Endpoints ============================================================>


        [Authorize]
        // Get all products and their stock levels
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _inventoryService.GetProductsAsync();
            return Ok(products);
        }


        // API endpoint to get all products for the logged-in vendor grouped by sub-category
        [HttpGet("vendor/")]
        public async Task<IActionResult> GetProductsByVendor()
        {
            var vendorId = User.FindFirst("UserId")?.Value;

            var groupedProducts = await _inventoryService.GetProductsByVendorAndGroupBySubCategoryAsync(vendorId);


            if (groupedProducts == null || !groupedProducts.Any())
            {
                return NotFound(new { Message = "No products found for this vendor." });
            }

            return Ok(groupedProducts);
        }


        // Get a specific product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _inventoryService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        // Update a product's stock level
        [HttpPut("stock/update/{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] UpdateStockDto updateProductDto)
        {
            if (updateProductDto == null)
            {
                return BadRequest("UpdateStockDto cannot be null.");
            }

            var product = await _inventoryService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Update stock level
            await _inventoryService.UpdateProductAsync(id, updateProductDto.StockCount);
            return Ok(new ApiResponse<object>("Stock level updated successfully", updateProductDto));
        }

        // Delete a product if not part of a pending order
        [HttpDelete("delete/product/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var success = await _inventoryService.DeleteProductAsync(id);
            if (!success)
            {
                throw new ArgumentException("Cannot delete a product that is part of a pending order or does not exist.");
                // return Ok(new ApiResponse<object>("Cannot delete a product that is part of a pending order or does not exist."));
            }

            return Ok(new ApiResponse<object>("Product deleted successfully", id));
        }
    }
}
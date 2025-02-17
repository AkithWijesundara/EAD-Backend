/*
 * File: Product Controller
 * Author: Perera V. H. P.
 * Description: This file contains the endpoints for products.
 * Created: 07/10/2024
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
    public class ProductController : ControllerBase
    {

        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        //! ======================================================== Define API Endpoints ============================================================>

        //! Get all products
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {

            var response = await _productService.GetAll();
            return Ok(new ApiResponse<object>("Successful", response));
        }

        //! Create a product
        [Authorize(Roles = "Vendor, Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(Product product)
        {
            var userId = User.FindFirst("UserId")?.Value;
            var response = await _productService.Create(userId, product);
            return Ok(new ApiResponse<Product>("Create successful", response));
        }

        //! Update a product
        [Authorize(Roles = "Vendor, Admin")]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, UpdateProductDto product)
        {

            var response = await _productService.Update(id, product);
            return Ok(new ApiResponse<object>("Update successful", response));
        }

        //! Delete a product
        [Authorize(Roles = "Vendor, Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _productService.Delete(id);
            return Ok(new ApiResponse<Product>("Delete successful", response));
        }

        //! Get a product by id
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _productService.GetById(id);
            return Ok(new ApiResponse<Product>("Successful", response));
        }



    }
}
using EAD_Backend.DTOs;
using EAD_Backend.Models;
using EAD_Backend.OtherModels;
using EAD_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EAD_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService){
            _orderService=orderService;
        }

        //Define routes from here onwards

        //Create new order
        [Authorize(Roles = "Customer")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(OrderDto order)
        {
            var response = await _orderService.CreateOrder(order);
            return Ok(new ApiResponse<Order>("Create successfull",response));
        }

        //get all orders
        [Authorize(Roles = "CSR")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var response = await _orderService.GetAllOrders();
            return Ok(new ApiResponse<List<OrderDisplayDto>>("Recived successfull",response));
        }


        //get all orders for a vendor
        [Authorize (Roles = "Vendor, Admin")]
        [HttpGet("vendor")]
        public async Task<IActionResult> GetAllOrdersForVendor()
        {
            var vendorId = User.FindFirst("UserId")?.Value;
            var response = await _orderService.GetAllOrdersForVendor(vendorId);
            return Ok(new ApiResponse<List<OrderDisplayDto>>("Recived successfull",response));
            }

        //Get customer order history
        [Authorize(Roles = "Customer")]
        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetOrderHistory(String id)
        {
            var response = await _orderService.GetOrderHistory(id);

            if(!response.Success)
            {
                if(response.Errors.Count != 0)
                {
                    return BadRequest(new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));
                }

                return StatusCode(500, new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));
            }

            return Ok(new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));

        }

        //Get all orders for vendor

        [Authorize(Roles = "Vendor")]//change to vendor later
        [HttpGet("orders/")]
        public async Task<IActionResult> GetVendorOrder()
        {
            var id = User.FindFirst("UserId")?.Value;
            var response = await _orderService.GetVendorOrder(id);

            if(!response.Success)
            {
                if(response.Errors.Count != 0)
                {
                    return BadRequest(new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));
                }

                return StatusCode(500, new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));
            }

            return Ok(new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));


        }

        //update oder details
        [Authorize(Roles = "Customer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(OrderUpdateDto updateOrderDto)
        {
            var response = await _orderService.UpdateOrder(updateOrderDto);
            
            if(!response.Success)
            {
                if(response.Errors.Count != 0)
                {
                    return BadRequest(new ApiResponse<List<String>>(response.Message,response.Errors));
                }

                return StatusCode(500, new ApiResponse<object>(response.Message));
            }

            return Ok(new ApiResponse<object>(response.Message));

        }

        //Cancel order request
        [Authorize(Roles = "Customer")]
        [HttpPut("request/{id}")]
        public async Task<IActionResult> CancelOrderRequest(String orderId)
        {
            var response = await _orderService.CancelOrderRequest(orderId);
            if(!response.Success)
            {
                if(response.Errors.Count != 0)
                {
                    return BadRequest(new ApiResponse<object>(response.Message,response.Errors));
                }

                return StatusCode(500, new ApiResponse<object>(response.Message));
            }

            return Ok(new ApiResponse<object>(response.Message));

        }

        //cancel order
        [Authorize(Roles = "CSR")]//change to csr later
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(OrderCancelDto orderCancelDto)
        {
            var response = await _orderService.CancelOrder(orderCancelDto);

             if(!response.Success)
            {
                if(response.Errors.Count != 0)
                {
                    return BadRequest(new ApiResponse<object>(response.Message,response.Errors));
                }

                return StatusCode(500, new ApiResponse<object>(response.Message));
            }

            return Ok(new ApiResponse<object>(response.Message));
        }

        //Get cancell requested orders
        [Authorize(Roles = "CSR")]//change to csr later
        [HttpGet("getCancell")]
        public async Task<IActionResult> GetCancelRequests()
        {
            var response = await _orderService.GetCancelleRequestOrders();

            if(!response.Success)
            {
                if(response.Errors.Count != 0)
                {
                    return BadRequest(new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));
                }

                return StatusCode(500, new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));
            }

            return Ok(new ApiResponse<object>(response.Success,response.Message,response.Data,response.Errors));

        }
    }

    
}
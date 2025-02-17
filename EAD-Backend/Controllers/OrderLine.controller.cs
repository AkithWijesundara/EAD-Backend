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
    public class OrderLineController : ControllerBase
    {
        private readonly OrderLineService _orderLineService;

        public OrderLineController(OrderLineService orderLineService)
        {
            _orderLineService=orderLineService;
        }

        //Define all routes here


    

    //! ======================================================== Define API Endpoints ============================================================>
    // update the order line status
    /* [Authorize(Roles = "Vendor, Admin")] */
    [HttpPut("update/status/{id}")]
    public async Task<IActionResult> UpdateOrderLineStatus(string id, [FromBody] OrderStatusUpdateDto orderLineStatus)
    {
        await _orderLineService.UpdateOrderLineStatusAsync(id, orderLineStatus.Status);
        return Ok(new ApiResponse<object>("Order Status Update successful"));
    }

     //get order line by vendor no
    [HttpGet("vendor/{vendorNo}")]
    public async Task<IActionResult> GetOrderLinesByVendor()
    {
        var vendorNo = User.FindFirst("UserId")?.Value;
        var orderLines = await _orderLineService.GetOrderLinesByVendor(vendorNo);
        return Ok(new ApiResponse<List<OrderLineDisplayDto>>("Received successfully", orderLines));
    }


}
}
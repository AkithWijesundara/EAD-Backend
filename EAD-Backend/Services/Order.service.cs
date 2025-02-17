using MongoDB.Driver;
using EAD_Backend.Models;
using EAD_Backend.DTOs;
using EAD_Backend.Data;
using EAD_Backend.OtherModels;
using ZstdSharp.Unsafe;

namespace EAD_Backend.Services
{
    public class OrderService
    {
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly OrderLineService _orderLineService;
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;
        private readonly EmailService _emailService;

        public OrderService(MongoDBService mongoDbService, OrderLineService orderLineService, NotificationService notificationService, UserService userService, EmailService emailService)
        {
            _ordersCollection = mongoDbService.Database?.GetCollection<Order>("Orders");
            _orderLineService = orderLineService;
            _userService = userService;
            _notificationService = notificationService;
            _emailService = emailService;
        }

        // Create a new order
        public async Task<Order> CreateOrder(OrderDto orderDto)
        {
            try
            {
                string generatedOrderNo = GenerateUniqueOrderNo();

                var order = new Order
                {
                    OrderNo = generatedOrderNo,
                    CustomerNo = orderDto.CustomerNo,
                    DeliveryAddress = orderDto.DeliveryAddress,
                    OrderDate = orderDto.OrderDate,
                    Status = orderDto.Status,
                    Comments = orderDto.Comments,
                    IsCancelRequested = false
                };

                await _ordersCollection.InsertOneAsync(order);



                // Create associated order lines
                foreach (var orderLineDto in orderDto.OrderLines)
                {
                    orderLineDto.OrderNo = generatedOrderNo; // Link order lines to the order
                    await _orderLineService.CreateOrderLine(orderLineDto);
                }

                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order: {ex.Message}");
                throw;
            }
        }

        //method to generate unique order no
        private string GenerateUniqueOrderNo()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string base36OrderNo = Base36Encode(timestamp);

            return $"ORD-{base36OrderNo}";
        }

        // Helper method to encode a number into Base36
        private string Base36Encode(long value)
        {
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var result = new Stack<char>();

            while (value > 0)
            {
                result.Push(chars[(int)(value % 36)]);
                value /= 36;
            }

            return new string(result.ToArray());
        }


        //Get all orders(for CSR)

        public async Task<List<OrderDisplayDto>> GetAllOrders()
        {
            try
            {
                var orders = await _ordersCollection.Find(_ => true).ToListAsync();

                var orderDtos = new List<OrderDisplayDto>();

                foreach (var order in orders)
                {
                    var orderLines = await _orderLineService.getOrderLineByOrderNo(order.OrderNo);

                    var customer = await _userService.GetById(order.CustomerNo);
                    var customerName = customer != null ? customer.Name : "Unknown";

                    var orderDto = new OrderDisplayDto
                    {
                        OrderId = order.OrderId,
                        OrderNo = order.OrderNo,
                        CustomerNo = order.CustomerNo,
                        CustomerName = customerName,
                        DeliveryAddress = order.DeliveryAddress,
                        OrderDate = order.OrderDate,
                        Status = order.Status,
                        Comments = order.Comments,
                        OrderLines = orderLines
                    };

                    orderDtos.Add(orderDto);
                }

                return orderDtos;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Getting order: {ex.Message}");
                throw;
            }
        }

        //get customer order history
        public async Task<ApiResponse<List<OrderDisplayDto>>> GetOrderHistory(String customerNo)
        {
            var response = new ApiResponse<List<OrderDisplayDto>>();

            try
            {
                var filter = Builders<Order>.Filter.Eq(o => o.CustomerNo, customerNo);

                var orders = await _ordersCollection.Find(filter).ToListAsync();


                if (orders.Count == 0)
                {
                    Console.WriteLine("inside");
                    response.Success = false;
                    response.Message = "No order history";
                    response.Errors.Add("No orders");
                    return response;
                }

                var orderDtos = new List<OrderDisplayDto>();

                foreach (var order in orders)
                {
                    var orderLines = await _orderLineService.getOrderLineByOrderNo(order.OrderNo);

                    var orderDto = new OrderDisplayDto
                    {
                        OrderId = order.OrderId,
                        OrderNo = order.OrderNo,
                        CustomerNo = order.CustomerNo,
                        DeliveryAddress = order.DeliveryAddress,
                        OrderDate = order.OrderDate,
                        Status = order.Status,
                        Comments = order.Comments,
                        OrderLines = orderLines
                    };

                    orderDtos.Add(orderDto);

                }

                if (orderDtos.Count > 0)
                {
                    response.Success = true;
                    response.Message = "Recived successfully";
                    response.Data = orderDtos;
                    return response;
                }
                else
                {
                    Console.WriteLine("inside");
                    response.Success = false;
                    response.Message = "No order history";
                    response.Errors.Add("No orders");
                    return response;
                }


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Server error: {ex.Message}";
                return response;
            }

        }

        //Get all orders for vendor

        public async Task<ApiResponse<List<OrderDisplayDto>>> GetVendorOrder(String vendorNo)
        {
            var response = new ApiResponse<List<OrderDisplayDto>>();

            try
            {

                var orders = await _ordersCollection.Find(_ => true).ToListAsync();

                if (orders.Count == 0)
                {
                    response.Success = false;
                    response.Message = "No orders yet";
                    response.Errors.Add("No orders");
                    return response;
                }

                var orderDtos = new List<OrderDisplayDto>();

                foreach (var order in orders)
                {
                    //check the for the given order no whether the vendorno is equal in order lines if true return the order lines

                    var orderLines = await _orderLineService.GetOrderLinesForVendor(order.OrderNo, vendorNo);

                    var customer = await _userService.GetById(order.CustomerNo);
                    var customerName = customer != null ? customer.Name : "Unknown";

                    if (orderLines.Count > 0)
                    {
                        var orderDto = new OrderDisplayDto
                        {
                            OrderId = order.OrderId,
                            OrderNo = order.OrderNo,
                            CustomerNo = order.CustomerNo,
                            CustomerName = customerName,
                            DeliveryAddress = order.DeliveryAddress,
                            OrderDate = order.OrderDate,
                            Status = order.Status,
                            Comments = order.Comments,
                            OrderLines = orderLines
                        };

                        orderDtos.Add(orderDto);
                    }

                }

                if (orderDtos.Count > 0)
                {
                    response.Success = true;
                    response.Message = "Recived successfully";
                    response.Data = orderDtos;
                    return response;
                }
                else
                {
                    response.Success = false;
                    response.Message = "No orders yet";
                    response.Errors.Add("No orders");
                    return response;
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Server error: {ex.Message}";
                return response;
            }
        }

        //Update order details(only allowed to update the addres, quantity and remove items)

        public async Task<ApiResponse<Order>> UpdateOrder(OrderUpdateDto orderUpdateDto)
        {
            var response = new ApiResponse<Order>();
            try
            {

                var order = await _ordersCollection.Find(o => o.OrderId == orderUpdateDto.OrderId).FirstOrDefaultAsync();

                if (order == null)
                {
                    response.Success = false;
                    response.Message = $"Order with order id {orderUpdateDto.OrderId} not found";
                    response.Errors.Add("Order Not found");
                    return response;
                }

                if (order.Status == "Dispatched")
                {
                    response.Success = false;
                    response.Message = "Order is already dispatched";
                    response.Errors.Add("Order is already dispatched");
                    return response;
                }

                foreach (var updateLine in orderUpdateDto.OrderLines)
                {
                    if (updateLine.Remove)
                    {
                        var removed = await _orderLineService.RemoveOrderLine(updateLine.OrderLineNo);
                        if (!removed)
                        {
                            response.Errors.Add("Order line could not be removed");
                        }
                    }
                    else if (updateLine.Qty.HasValue)
                    {
                        var updated = await _orderLineService.UpdateOrderLineQty(updateLine.OrderLineNo, updateLine.Qty);
                        if (!updated)
                        {
                            response.Errors.Add("Order line could not be updated");
                        }
                    }

                }

                if (!order.DeliveryAddress.Equals(orderUpdateDto.DeliveryAddress))
                {
                    var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderUpdateDto.OrderId);
                    var update = Builders<Order>.Update.Set(o => o.DeliveryAddress, orderUpdateDto.DeliveryAddress);

                    var result = await _ordersCollection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Order> { ReturnDocument = ReturnDocument.After });

                    if (!result.DeliveryAddress.Equals(orderUpdateDto.DeliveryAddress))
                    {
                        response.Errors.Add("Delivery address could not be updated");
                    }
                }

                if (response.Errors.Count > 0)
                {
                    response.Success = false;
                    response.Message = "Update failed";
                    return response;
                }

                response.Success = true;
                response.Message = "Update successfull";
                return response;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Server error: {ex.Message}";
                return response;
            }

        }

        // Request to cancel order

        public async Task<ApiResponse<Order>> CancelOrderRequest(String orderId)
        {
            var response = new ApiResponse<Order>();

            try
            {
                var order = await _ordersCollection.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();

                if (order == null)
                {
                    response.Success = false;
                    response.Message = $"Order with order id {orderId} not found";
                    response.Errors.Add("Order Not found");
                    return response;
                }

                if (order.Status == "Dispatched")
                {
                    response.Success = false;
                    response.Message = "Order is already dispatched";
                    response.Errors.Add("Order is already dispatched");
                    return response;
                }

                var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
                var update = Builders<Order>.Update.Set(o => o.IsCancelRequested, true);

                var result = await _ordersCollection.FindOneAndUpdateAsync(
                    filter,
                    update,
                    new FindOneAndUpdateOptions<Order> { ReturnDocument = ReturnDocument.After }
                );

                response.Success = true;
                response.Message = "Order cancelation request sent";
                return response;


            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Server error: {ex.Message}";
                return response;
            }
        }

        //cancel order by csr

        public async Task<ApiResponse<Order>> CancelOrder(OrderCancelDto orderCancelDto)
        {
            var response = new ApiResponse<Order>();

            try
            {
                var order = await _ordersCollection.Find(o => o.OrderId == orderCancelDto.OrderId).FirstOrDefaultAsync();

                if (order == null)
                {
                    response.Success = false;
                    response.Message = $"Order with order id {orderCancelDto.OrderId} not found";
                    response.Errors.Add("Order Not found");
                    return response;
                }

                if (orderCancelDto.Comments == null)
                {
                    response.Success = false;
                    response.Message = "Please add a comment";
                    response.Errors.Add("Order cannot be canceled with out a comment");
                    return response;
                }

                if ((bool)order.IsCancelRequested)
                {
                    var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderCancelDto.OrderId);
                    var update = Builders<Order>.Update
                    .Set(o => o.Comments, orderCancelDto.Comments)
                    .Set(o => o.Status, "Cancelled");

                    var result = await _ordersCollection.FindOneAndUpdateAsync(
                        filter,
                        update,
                        new FindOneAndUpdateOptions<Order> { ReturnDocument = ReturnDocument.After }
                    );

                    var customer = await _userService.GetById(order.CustomerNo);
                    var customerEmail = customer != null ? customer.Email : "akithwijesundara@gmail.com";

                    var emailDto = new EmailDTO
                    {
                        Email = customerEmail,
                        Subject = "Order cancellation Supermart",
                        Message = $@"
                                <h1>Your Order is cancelled</h1>
                                <p>Cancelled!</p> 
                                <p>Your order is cancelled</p>"
                    };

                    await _emailService.SendEmailAsync(emailDto);

                    var notification = new Notification
                    {
                        Title = "Cancelation Alert",
                        Message = $"Your order {order.OrderNo} is cancelled as your request.",
                        UserId = order.CustomerNo
                    };

                    _notificationService.CreateNotificationAsync(notification);

                    response.Success = true;
                    response.Message = "Order canceled succesfully";
                    return response;

                }
                else
                {
                    response.Success = false;
                    response.Message = "Order cannot be canceled with out a user request";
                    response.Errors.Add("Order cannot be canceled");
                    return response;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in canceling the order {ex.Message}");
                response.Success = false;
                response.Message = $"Server error: {ex.Message}";
                return response;
            }
        }


        //Get all orders for a vendor(naveen)

        public async Task<List<OrderDisplayDto>> GetAllOrdersForVendor(string vendorId)
        {
            try
            {
                var orderLines = await _orderLineService.GetOrderLinesByVendor(vendorId);

                var orderIds = orderLines.Select(ol => ol.OrderNo).Distinct().ToList();

                var orders = await _ordersCollection.Find(o => orderIds.Contains(o.OrderNo)).ToListAsync();

                var orderDtos = new List<OrderDisplayDto>();

                foreach (var order in orders)
                {
                    var orderDto = new OrderDisplayDto
                    {
                        OrderId = order.OrderId,
                        OrderNo = order.OrderNo,
                        CustomerNo = order.CustomerNo,
                        DeliveryAddress = order.DeliveryAddress,
                        OrderDate = order.OrderDate,
                        Status = order.Status,
                        Comments = order.Comments,
                        OrderLines = orderLines.Where(ol => ol.OrderNo == order.OrderNo).ToList()
                    };

                    orderDtos.Add(orderDto);
                }

                return orderDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Getting order: {ex.Message}");
                throw;
            }
        }

        //Get cancell requested orders
        public async Task<ApiResponse<List<OrderDisplayDto>>> GetCancelleRequestOrders()
        {
            var response = new ApiResponse<List<OrderDisplayDto>>();

            try
            {
                var filter = Builders<Order>.Filter.And(
                    Builders<Order>.Filter.Eq(o => o.IsCancelRequested, true),
                    Builders<Order>.Filter.Ne(o => o.Status, "Cancelled")
                );

                var orders = await _ordersCollection.Find(filter).ToListAsync();

                if (orders.Count == 0)
                {
                    response.Success = true;
                    response.Message = $"No orders to be cancelled";
                    return response;
                }

                var orderDtos = new List<OrderDisplayDto>();

                foreach (var order in orders)
                {

                    var customer = await _userService.GetById(order.CustomerNo);
                    var customerName = customer != null ? customer.Name : "Unknown";

                    var orderDto = new OrderDisplayDto
                    {
                        OrderId = order.OrderId,
                        OrderNo = order.OrderNo,
                        CustomerNo = order.CustomerNo,
                        CustomerName = customerName,
                        DeliveryAddress = order.DeliveryAddress,
                        OrderDate = order.OrderDate,
                        Status = order.Status,
                        Comments = order.Comments,
                    };

                    orderDtos.Add(orderDto);
                }

                response.Success = true;
                response.Data = orderDtos;
                response.Message = "Orders to be cancelled";
                return response;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Getting order: {ex.Message}");
                throw;
            }
        }





    }
}
using MongoDB.Driver;
using EAD_Backend.Models;
using EAD_Backend.DTOs;
using EAD_Backend.Data;
using MongoDB.Bson;

namespace EAD_Backend.Services
{
    public class OrderLineService
    {
        private readonly IMongoCollection<OrderLine> _orderLinesCollection;
        private readonly IMongoCollection<Order> _ordersCollection;
        private readonly ProductService _productService;
        private readonly UserService _userService;
        private readonly InventoryService _inventoryService;
        private readonly IMongoCollection<Product> _productsCollection;
        private readonly IMongoCollection<Vendor> _vendorsCollection;

        public OrderLineService(MongoDBService mongoDbService, ProductService productService, UserService userService, InventoryService inventoryService)
        {
            _orderLinesCollection = mongoDbService.Database?.GetCollection<OrderLine>("OrderLines");
            _ordersCollection = mongoDbService.Database?.GetCollection<Order>("Orders");
            _productsCollection = mongoDbService.Database?.GetCollection<Product>("Products");
            _vendorsCollection = mongoDbService.Database?.GetCollection<Vendor>("Vendors");
            _productService = productService;
            _userService = userService;
            _inventoryService = inventoryService;
        }

        // Create a new order line
        public async Task CreateOrderLine(OrderLineDto orderLineDto)
        {
            try
            {
                var orderLine = new OrderLine
                {
                    OrderLineNo = orderLineDto.OrderLineNo,
                    ProductNo = orderLineDto.ProductNo,
                    VendorNo = orderLineDto.VendorNo,
                    OrderNo = orderLineDto.OrderNo,
                    Status = orderLineDto.Status,
                    Qty = orderLineDto.Qty,
                    UnitPrice = orderLineDto.UnitPrice,
                    Total = orderLineDto.Total
                };

                await _orderLinesCollection.InsertOneAsync(orderLine);

                // Decrease stock count for the product in the inventory
                var product = await _inventoryService.GetProductByIdAsync(orderLine.ProductNo);
                if (product != null)
                {
                    int newStockCount = product.StockCount - orderLine.Qty;

                    if(newStockCount < 0){
                        throw new Exception("Stock count cannot be negative");
                    }

                    // Update the product stock
                    await _inventoryService.UpdateProductAsync(orderLine.ProductNo, newStockCount);

                    // Check if the stock is low and notify the vendor
                    if (newStockCount <= product.LowStockThreshold)
                    {
                        _inventoryService.NotifyVendor(product.VendorId, product.Name, newStockCount, product.Id);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating order line: {ex.Message}");
                throw;
            }
        }

        //get order line by order no

        public async Task<List<OrderLineDisplayDto>> getOrderLineByOrderNo(String orderNo)
        {
            try
            {
                var orderLines = await _orderLinesCollection.Find(ol => ol.OrderNo == orderNo).ToListAsync();

                var orderLineDtos = new List<OrderLineDisplayDto>();

                foreach (var ol in orderLines)
                {
                    var product = await _productService.GetById(ol.ProductNo);
                    var productName = product != null ? product.Name : "Unknown Product";

                    var vendor = await _userService.GetById(ol.VendorNo);
                    var vendorName = vendor != null ? vendor.Name : "Unknown Vendor";

                    var orderLineDto = new OrderLineDisplayDto
                    {
                        OrderLineNo = ol.OrderLineNo,
                        ProductNo = ol.ProductNo,
                        VendorNo = ol.VendorNo,
                        OrderNo = ol.OrderNo,
                        Status = ol.Status,
                        Qty = ol.Qty,
                        UnitPrice = ol.UnitPrice,
                        Total = ol.Total,
                        ProductName = productName,
                        VendorName = vendorName

                    };

                    orderLineDtos.Add(orderLineDto);
                }


                return orderLineDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving order lines for OrderNo {orderNo}: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateOrderLineQty(String orderLineNo, int? newQty)
        {
            try
            {

                var orderLine = await _orderLinesCollection.Find(ol => ol.OrderLineNo == orderLineNo).FirstOrDefaultAsync();
                if (orderLine == null)
                {
                    Console.WriteLine($"Orderline with order no {orderLineNo} not found");
                    return false;
                }

                orderLine.Qty = (int)newQty;

                await _orderLinesCollection.FindOneAndReplaceAsync(ol => ol.OrderLineNo == orderLineNo, orderLine);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when updating the order line {ex.Message}");
                return false;

            }
        }

        public async Task<bool> RemoveOrderLine(String orderLineNo)
        {
            try
            {
                var result = await _orderLinesCollection.DeleteOneAsync(ol => ol.OrderLineNo == orderLineNo);

                if (result.DeletedCount == 0)
                {
                    Console.WriteLine($"Order line not deleted");
                    return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when removing order line {ex.Message}");
                return false;
            }
        }


        public async Task<List<OrderLineDisplayDto>> GetOrderLinesForVendor(String orderNo, String vendorNo)
        {
            try
            {
                var filter = Builders<OrderLine>.Filter.And(
                Builders<OrderLine>.Filter.Eq(ol => ol.OrderNo, orderNo),
                Builders<OrderLine>.Filter.Eq(ol => ol.VendorNo, vendorNo)
                );

                var orderLines = await _orderLinesCollection.Find(filter).ToListAsync();

                var orderLineDtos = new List<OrderLineDisplayDto>();

                foreach (var ol in orderLines)
                {
                    var product = await _productService.GetById(ol.ProductNo);
                    var productName = product != null ? product.Name : "Unknown Product";

                    var vendor = await _userService.GetById(ol.VendorNo);
                    var vendorName = vendor != null ? vendor.Name : "Unknown Vendor";

                    var orderLineDto = new OrderLineDisplayDto
                    {
                        OrderLineNo = ol.OrderLineNo,
                        ProductNo = ol.ProductNo,
                        VendorNo = ol.VendorNo,
                        OrderNo = ol.OrderNo,
                        Status = ol.Status,
                        Qty = ol.Qty,
                        UnitPrice = ol.UnitPrice,
                        Total = ol.Total,
                        ProductName = productName,
                        VendorName = vendorName
                    };

                    orderLineDtos.Add(orderLineDto);
                }


                return orderLineDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving order lines for OrderNo {orderNo}: {ex.Message}");
                throw;
            }
        }



        //update order line status and check if all order lines are delivered
        public async Task UpdateOrderLineStatusAsync(string orderLineNo, string newStatus)
        {

            var filterOrderLine = Builders<OrderLine>.Filter.Eq(ol => ol.OrderLineNo, orderLineNo);
            var updateOrderLine = Builders<OrderLine>.Update.Set(ol => ol.Status, newStatus);
            await _orderLinesCollection.UpdateOneAsync(filterOrderLine, updateOrderLine);
            var updatedOrderLine = await _orderLinesCollection.Find(filterOrderLine).FirstOrDefaultAsync();

            if (updatedOrderLine == null)
            {
                throw new Exception("OrderLine not found.");
            }

            var orderNo = updatedOrderLine.OrderNo;

            var allOrderLinesDelivered = await _orderLinesCollection
                .Find(ol => ol.OrderNo == orderNo && ol.Status != "Delivered")
                .AnyAsync() == false;

            if (allOrderLinesDelivered)
            {
                var filterOrder = Builders<Order>.Filter.Eq(o => o.OrderNo, orderNo);
                var updateOrder = Builders<Order>.Update.Set(o => o.Status, "Delivered");

                // Check if Order exists before updating
                var order = await _ordersCollection.Find(filterOrder).FirstOrDefaultAsync();
                if (order == null)
                {
                    throw new Exception($"Order with OrderNo {orderNo} not found.");
                }


                await _ordersCollection.UpdateOneAsync(filterOrder, updateOrder);
                return;
            }
            else
            {
                var partiallyDeliveredOrderLines = await _orderLinesCollection
                    .Find(ol => ol.OrderNo == orderNo && ol.Status == "Delivered")
                    .AnyAsync();

                if (partiallyDeliveredOrderLines)
                {

                    var filterOrder = Builders<Order>.Filter.Eq(o => o.OrderNo, orderNo);
                    var order = await _ordersCollection.Find(filterOrder).FirstOrDefaultAsync();
                    if (order == null)
                    {
                        throw new Exception($"Order with OrderNo {orderNo} not found.");
                    }

                    Console.WriteLine("Updating Order status to Partially Delivered");
                    await _ordersCollection.UpdateOneAsync(filterOrder, Builders<Order>.Update.Set(o => o.Status, "Partially Delivered"));
                    return;
                }
            }
        }


        //get order line by vendor no
        public async Task<List<OrderLineDisplayDto>> GetOrderLinesByVendor(String vendorNo)
        {
            try
            {
                var orderLines = await _orderLinesCollection.Find(ol => ol.VendorNo == vendorNo).ToListAsync();

                var orderLineDtos = new List<OrderLineDisplayDto>();

                foreach (var ol in orderLines)
                {
                    //remove these two and replace with the relevent service function calls
                    var product = await _productsCollection.Find(p => p.Id == ol.ProductNo).FirstOrDefaultAsync();
                    var productName = product != null ? product.Name : "Unknown Product";

                    var vendor = await _vendorsCollection.Find(v => v.Id == ol.VendorNo).FirstOrDefaultAsync();
                    var vendorName = vendor != null ? vendor.Name : "Unknown Vendor";

                    var orderLineDto = new OrderLineDisplayDto
                    {
                        OrderLineNo = ol.OrderLineNo,
                        ProductNo = ol.ProductNo,
                        VendorNo = ol.VendorNo,
                        OrderNo = ol.OrderNo,
                        Status = ol.Status,
                        Qty = ol.Qty,
                        UnitPrice = ol.UnitPrice,
                        Total = ol.Total,
                        ProductName = productName,

                    };

                    orderLineDtos.Add(orderLineDto);
                }

                return orderLineDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving order lines for VendorNo {vendorNo}: {ex.Message}");
                throw;

            }
        }

    }




}


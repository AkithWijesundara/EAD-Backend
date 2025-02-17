namespace EAD_Backend.DTOs
{
    public class OrderDto
    {
        public string OrderId {get; set;} = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public string CustomerNo { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Comments { get; set; } = string.Empty;
        public List<OrderLineDto> OrderLines { get; set; } = new(); // For associated order lines
    }

}

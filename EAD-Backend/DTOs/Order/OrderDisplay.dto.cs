namespace EAD_Backend.DTOs
{
    public class OrderDisplayDto
    {
        public string OrderId {get; set;} = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public string CustomerNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Comments { get; set; }
        public List<OrderLineDisplayDto> OrderLines { get; set; } = new(); // For associated order lines
    }

}
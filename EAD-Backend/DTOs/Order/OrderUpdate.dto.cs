namespace EAD_Backend.DTOs
{
    public class OrderUpdateDto
    {
        public string OrderId {get; set;} = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;

        public List<OrderLineUpdateDto> OrderLines { get; set; } = new(); // For associated order lines
    }

}
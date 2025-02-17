namespace EAD_Backend.DTOs
{
    public class OrderCancelDto
    {
        public string OrderId {get; set;} = string.Empty;
        public string? Comments { get; set; }
    }

}

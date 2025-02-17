namespace EAD_Backend.DTOs
{
    public class OrderLineUpdateDto
    {
       public string OrderLineNo { get; set; } = string.Empty;
        public int? Qty { get; set; }
        public bool Remove { get; set; } = false; 
    }

}
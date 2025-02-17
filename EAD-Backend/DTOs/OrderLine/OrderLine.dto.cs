namespace EAD_Backend.DTOs
{
    public class OrderLineDto
    {
        public string OrderLineNo { get; set; } = string.Empty;
        public string ProductNo { get; set; } = string.Empty;

        public string OrderNo {get; set; } = string.Empty;
        
        public string VendorNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Qty { get; set; }
        public float UnitPrice { get; set; }
        public float Total { get; set; }
    }
}

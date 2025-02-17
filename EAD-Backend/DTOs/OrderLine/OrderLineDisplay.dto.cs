namespace EAD_Backend.DTOs
{
    public class OrderLineDisplayDto
    {
        public string OrderLineNo { get; set; } = string.Empty;
        public string ProductNo { get; set; } = string.Empty;
        public string VendorNo { get; set; } = string.Empty;
        public string OrderNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Qty { get; set; }
        public float UnitPrice { get; set; }
        public float Total { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
    }
}

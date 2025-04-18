namespace BookStore.Application.DTOs
{
    public class ReportDto
    {
        public int ReportID { get; set; }
        public int OrderID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerReviews { get; set; }
        public string CustomerName { get; set; }
    }
}

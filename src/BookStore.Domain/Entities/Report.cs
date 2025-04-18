namespace BookStore.Domain.Entities
{
    public class Report
    {
        public int ReportID { get; set; }
        public int OrderID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerReviews { get; set; }

        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}

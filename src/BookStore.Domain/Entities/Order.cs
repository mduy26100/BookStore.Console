namespace BookStore.Domain.Entities
{
    public class Order
    {
        public int OrderID { get; set; }
        public int? AccountID { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }

        public Account Account { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}

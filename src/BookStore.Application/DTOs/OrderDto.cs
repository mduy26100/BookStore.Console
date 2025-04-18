namespace BookStore.Application.DTOs
{
    public class OrderDto
    {
        public OrderDto()
        {
            OrderDetails = new List<OrderDetailDto>();
        }

        public int OrderID { get; set; }
        public int? AccountID { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public bool UseAccountInfo { get; set; } = true;
        public List<OrderDetailDto> OrderDetails { get; set; }
    }
}
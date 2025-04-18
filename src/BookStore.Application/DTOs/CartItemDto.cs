namespace BookStore.Application.DTOs
{
    public class CartItemDto
    {
        public int CartID { get; set; }
        public int AccountID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public BookDto Book { get; set; }
    }
}

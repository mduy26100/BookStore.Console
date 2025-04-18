namespace BookStore.Application.DTOs
{
    public class ShoppingCartDto
    {
        public ShoppingCartDto()
        {
            Items = new List<CartItemDto>();
        }

        public int CartID { get; set; }
        public int AccountID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public BookDto Book { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal TotalAmount => Items?.Sum(i => i.Book.Price * i.Quantity) ?? 0;
    }
}

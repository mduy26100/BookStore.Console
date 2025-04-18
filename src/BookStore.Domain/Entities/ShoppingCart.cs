namespace BookStore.Domain.Entities
{
    public class ShoppingCart
    {
        public int CartID { get; set; }
        public int AccountID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }

        public Account Account { get; set; }
        public Book Book { get; set; }
    }
}

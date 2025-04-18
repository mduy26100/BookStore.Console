namespace BookStore.Domain.Entities
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }

        public ICollection<BookCategory> BookCategories { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }
        public ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public ICollection<Report> Reports { get; set; }
    }
}

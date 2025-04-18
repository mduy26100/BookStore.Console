namespace BookStore.Domain.Entities
{
    public class BookCategory
    {
        public int BookID { get; set; }
        public int CategoryID { get; set; }

        public Book Book { get; set; }
        public Category Category { get; set; }
    }
}

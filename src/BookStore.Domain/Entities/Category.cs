namespace BookStore.Domain.Entities
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<BookCategory> BookCategories { get; set; }
    }
}

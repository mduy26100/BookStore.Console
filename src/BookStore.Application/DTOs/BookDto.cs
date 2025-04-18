namespace BookStore.Application.DTOs
{
    public class BookDto
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string Description { get; set; }
        public List<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public List<ReportDto> Reports { get; set; } = new List<ReportDto>();
    }
}

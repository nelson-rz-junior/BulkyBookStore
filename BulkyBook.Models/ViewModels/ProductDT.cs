#nullable disable
namespace BulkyBook.Models.ViewModels
{
    public class ProductDT
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string ISBN { get; set; }

        public string Author { get; set; }

        public string CategoryName { get; set; }

        public decimal Price { get; set; }
    }
}

namespace BulkyBook.Models.ViewModels
{
    public class OrderConfirmationVM
    {
        public OrderHeader OrderHeader { get; set; }

        public IEnumerable<OrderDetail> Items { get; set; }
    }
}

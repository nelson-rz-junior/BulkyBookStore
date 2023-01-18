using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);

        Task UpdateStatus(int orderId, string orderStatus, string paymentStatus = null);

        Task UpdateStripeSessionId(int orderId, string sessionId, string paymentIntentId);
    }
}

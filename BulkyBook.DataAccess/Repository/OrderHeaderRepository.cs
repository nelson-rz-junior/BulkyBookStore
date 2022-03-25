#nullable disable
using BulkyBook.DataAccess.Context;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }

        public async Task UpdateStatus(int orderId, string orderStatus, string paymentStatus = null)
        {
            var orderHeader = await _context.OrderHeaders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.OrderStatus = orderStatus;

                if (paymentStatus != null)
                {
                    orderHeader.PaymentStatus = paymentStatus;
                }
            }
        }

        public async Task UpdateStripeSessionId(int orderId, string sessionId, string paymentIntentId)
        {
            var orderHeader = await _context.OrderHeaders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (orderHeader != null)
            {
                orderHeader.SessionId = sessionId;
                orderHeader.PaymentIntentId = paymentIntentId;
            }
        }
    }
}

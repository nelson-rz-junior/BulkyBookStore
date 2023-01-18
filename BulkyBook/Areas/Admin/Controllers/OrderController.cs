using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Linq.Expressions;
using System.Security.Claims;

namespace BulkyBook.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly StripeSettings _options;

		public OrderController(IUnitOfWork unitOfWork, IOptions<StripeSettings> options)
        {
            _unitOfWork = unitOfWork;
			_options = options.Value;
        }

        [BindProperty]
        public OrderConfirmationVM OrderVM { get; set; }

        public IActionResult Index(string status)
        {
            ViewData["Status"] = status;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(o => o.Id == orderId, includeProperties: "ApplicationUser"),
                Items = await _unitOfWork.OrderDetailRepository.GetAllAsync(s => s.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        public async Task<IActionResult> Details()
        {
            var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(o => o.Id == OrderVM.OrderHeader.Id, tracked: false);
            if (orderHeader != null)
            {
                orderHeader.Name = OrderVM.OrderHeader.Name;
				orderHeader.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
				orderHeader.StreetAddress = OrderVM.OrderHeader.StreetAddress;
				orderHeader.City = OrderVM.OrderHeader.City;
				orderHeader.State = OrderVM.OrderHeader.State;
				orderHeader.PostalCode = OrderVM.OrderHeader.PostalCode;

                if (OrderVM.OrderHeader.Carrier != null)
                {
                    orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
                }

				if (OrderVM.OrderHeader.TrackingNumber != null)
				{
					orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
				}

                _unitOfWork.OrderHeaderRepository.Update(orderHeader);
                await _unitOfWork.SaveChangesAsync();

				TempData["SuccessMessage"] = $"Order #{orderHeader.Id} updated successfully";
			}

			return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

		[HttpPost]
        [Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
		public async Task<IActionResult> StartProcessing()
		{
			await _unitOfWork.OrderHeaderRepository.UpdateStatus(OrderVM.OrderHeader.Id, SD.STATUS_IN_PROCESS);
			await _unitOfWork.SaveChangesAsync();

			TempData["SuccessMessage"] = $"Order in process successfully";

			return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
		}

		[HttpPost]
		[Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
		public async Task<IActionResult> ShipOrder()
		{
			var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(o => o.Id == OrderVM.OrderHeader.Id, tracked: false);
            if (orderHeader != null)
            {
                orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
				orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
                orderHeader.OrderStatus = SD.STATUS_SHIPPED;
                orderHeader.ShippingDate = DateTime.Now;

                if (orderHeader.PaymentStatus == SD.PAYMENT_STATUS_DELAYED)
                {
                    orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                }

				_unitOfWork.OrderHeaderRepository.Update(orderHeader);
				await _unitOfWork.SaveChangesAsync();

				TempData["SuccessMessage"] = $"Order shipped successfully";
			}

			return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
		}

		[HttpPost]
		public async Task<IActionResult> PayNow()
		{
			int orderId = OrderVM.OrderHeader.Id;

			OrderVM.OrderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(o => o.Id == orderId, includeProperties: "ApplicationUser");
			OrderVM.Items = await _unitOfWork.OrderDetailRepository.GetAllAsync(s => s.OrderHeaderId == orderId, includeProperties: "Product");

			var lineItems = new List<SessionLineItemOptions>();

			foreach (var item in OrderVM.Items)
			{
				// Stripe item
				lineItems.Add(new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.UnitPrice * 100),
						Currency = "brl",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Title,
							Description = $"{item.Product.Description.Substring(0, 100)}..."
						}
					},
					Quantity = item.Quantity
				});
			}

			// Stripe session settings
			var options = new SessionCreateOptions
			{
				CustomerEmail = OrderVM.OrderHeader.ApplicationUser.Email,
				LineItems = lineItems,
				Mode = "payment",
				SuccessUrl = $"{_options.PaymentConfirmationUrl}?orderId={orderId}",
				CancelUrl = $"{_options.PaymentCancelUrl}?orderId={orderId}"
			};

			var service = new SessionService();
			Session session = service.Create(options);

			await _unitOfWork.OrderHeaderRepository.UpdateStripeSessionId(orderId, session.Id, session.PaymentIntentId);
			await _unitOfWork.SaveChangesAsync();

			Response.Headers.Add("Location", session.Url);

			return new StatusCodeResult(303);
		}

		[HttpPost]
		[Authorize(Roles = $"{SD.ROLE_USER_ADMIN},{SD.ROLE_USER_EMPLOYEE}")]
		public async Task<IActionResult> CancelOrder()
		{
			var orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(o => o.Id == OrderVM.OrderHeader.Id, tracked: false);
			if (orderHeader != null)
			{
				if (orderHeader.PaymentStatus == SD.PAYMENT_STATUS_APPROVED)
                {
                    // Refund
                    var options = new RefundCreateOptions
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeader.PaymentIntentId
                    };

                    var service = new RefundService();
                    Refund refund = await service.CreateAsync(options);

                    await _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, SD.STATUS_CANCELLED, SD.STATUS_REFUNDED);
                }
				else
				{
					await _unitOfWork.OrderHeaderRepository.UpdateStatus(orderHeader.Id, SD.STATUS_CANCELLED, SD.STATUS_CANCELLED);
				}
			}
            
			await _unitOfWork.SaveChangesAsync();

			TempData["SuccessMessage"] = $"Order cancelled successfully";

			return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
		}

		[HttpGet]
		public async Task<IActionResult> PaymentConfirmation(int orderId)
		{
			OrderHeader orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(o => o.Id == orderId);
			if (orderHeader.PaymentStatus == SD.PAYMENT_STATUS_DELAYED)
			{
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					await _unitOfWork.OrderHeaderRepository.UpdateStatus(orderId, orderHeader.OrderStatus, SD.PAYMENT_STATUS_APPROVED);
					await _unitOfWork.SaveChangesAsync();
				}
			}

			return View(orderId);
		}

		#region API Calls

		[HttpGet("api/orders/{status}")]
        public async Task<IActionResult> GetOrders(string status)
        {
            Expression<Func<OrderHeader, bool>> filter = status switch
            {
                "pending" => o => o.PaymentStatus == SD.PAYMENT_STATUS_DELAYED,
                "inprocess" => o => o.OrderStatus == SD.STATUS_IN_PROCESS,
                "completed" => o => o.OrderStatus == SD.STATUS_SHIPPED,
                "approved" => o => o.OrderStatus == SD.STATUS_APPROVED,
                _ => o => true
            };

            if (User.IsInRole(SD.ROLE_USER_INDIVIDUAL) || User.IsInRole(SD.ROLE_USER_COMPANY))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                filter = filter.And(o => o.ApplicationUserId == claims.Value);
            }

            return Json(new 
            { 
                data = await _unitOfWork.OrderHeaderRepository.GetAllAsync(filter, includeProperties: "ApplicationUser")
            });
        }

        #endregion
    }
}

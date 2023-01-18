using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("[controller]")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly StripeSettings _options;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IOptions<StripeSettings> options)
        {
            _unitOfWork = unitOfWork;
            _options = options.Value;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new()
            {
                OrderHeader = new(),
                Items = await _unitOfWork.ShoppingCartRepository.GetAllAsync(s => s.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.Items.Sum(s => s.FinalPrice);

            return View(ShoppingCartVM);
        }

        [HttpGet("Plus/{cartId}")]
        public async Task<IActionResult> Plus(int cartId)
        {
            var item = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(filter: s => s.Id == cartId, includeProperties: "Product");
            if (item != null)
            {
                _unitOfWork.ShoppingCartRepository.IncrementQuantity(item, 1);
                _unitOfWork.ShoppingCartRepository.SetUnitPrice(item);
                _unitOfWork.ShoppingCartRepository.SetFinalPrice(item);

                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Minus/{cartId}")]
        public async Task<IActionResult> Minus(int cartId)
        {
            var item = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(filter: s => s.Id == cartId, includeProperties: "Product");
            if (item != null)
            {
                if (item.Quantity == 1)
                {
                    _unitOfWork.ShoppingCartRepository.Remove(item);
                }
                else
                {
                    _unitOfWork.ShoppingCartRepository.DecrementQuantity(item, 1);
                    _unitOfWork.ShoppingCartRepository.SetUnitPrice(item);
                    _unitOfWork.ShoppingCartRepository.SetFinalPrice(item);
                }

                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Remove/{cartId}")]
        public async Task<IActionResult> Remove(int cartId)
        {
            var item = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(filter: s => s.Id == cartId);
            if (item != null)
            {
                _unitOfWork.ShoppingCartRepository.Remove(item);
                await _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new()
            {
                OrderHeader = new(),
                Items = await _unitOfWork.ShoppingCartRepository.GetAllAsync(s => s.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = await _unitOfWork.ApplicationUserRepository.GetFirstOrDefaultAsync(u => u.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.Items.Sum(s => s.FinalPrice);

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var emailAddress = claimsIdentity.FindFirst(ClaimTypes.Email);

            ApplicationUser applicationUser = await _unitOfWork.ApplicationUserRepository.GetFirstOrDefaultAsync(u => u.Id == claim.Value);
            bool isCustomerFlow = applicationUser.CompanyId.GetValueOrDefault() == 0;

            ShoppingCartVM.Items = await _unitOfWork.ShoppingCartRepository.GetAllAsync(s => s.ApplicationUserId == claim.Value, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.Items.Sum(s => s.FinalPrice);

            if (isCustomerFlow)
            {
                ShoppingCartVM.OrderHeader.OrderStatus = SD.STATUS_PENDING;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PAYMENT_STATUS_PENDING;
            }
            else
            {
                ShoppingCartVM.OrderHeader.OrderStatus = SD.STATUS_APPROVED;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PAYMENT_STATUS_DELAYED;
            }
            
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            await _unitOfWork.OrderHeaderRepository.AddAsync(ShoppingCartVM.OrderHeader);
            await _unitOfWork.SaveChangesAsync();

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in ShoppingCartVM.Items)
            {
                OrderDetail orderDetail = new()
                {
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    FinalPrice = item.FinalPrice
                };

                if (isCustomerFlow)
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
                
                await _unitOfWork.OrderDetailRepository.AddAsync(orderDetail);
            }

            await _unitOfWork.SaveChangesAsync();

            if (isCustomerFlow)
            {
                // Stripe session settings
                var options = new SessionCreateOptions
                {
                    CustomerEmail = emailAddress.Value,
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = $"{_options.SuccessUrl}/{ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = _options.CancelUrl
                };

                var service = new SessionService();
                Session session = service.Create(options);

                await _unitOfWork.OrderHeaderRepository.UpdateStripeSessionId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                await _unitOfWork.SaveChangesAsync();

                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(303);
            }

            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }

        [HttpGet("OrderConfirmation/{id}")]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            OrderHeader orderHeader = await _unitOfWork.OrderHeaderRepository.GetFirstOrDefaultAsync(o => o.Id == id);

            OrderConfirmationVM orderConfirmationVM = new()
            {
                OrderHeader = orderHeader,
                Items = await _unitOfWork.OrderDetailRepository.GetAllAsync(s => s.OrderHeaderId == orderHeader.Id, includeProperties: "Product")
            };

            if (orderHeader.PaymentStatus != SD.PAYMENT_STATUS_DELAYED)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _unitOfWork.OrderHeaderRepository.UpdateStatus(id, SD.STATUS_APPROVED, SD.PAYMENT_STATUS_APPROVED);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            // Clear the shopping cart
            var items = await _unitOfWork.ShoppingCartRepository.GetAllAsync(s => s.ApplicationUserId == orderHeader.ApplicationUserId);
            if (items.Any())
            {
                _unitOfWork.ShoppingCartRepository.RemoveRange(items);
                await _unitOfWork.SaveChangesAsync();
            }
            
            return View(orderConfirmationVM);
        }
    }
}

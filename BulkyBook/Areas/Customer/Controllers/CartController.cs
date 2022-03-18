#nullable disable
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("[controller]")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        [HttpGet("Summary")]
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
    }
}

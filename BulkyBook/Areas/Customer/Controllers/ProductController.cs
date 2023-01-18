using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> Index(int productId)
        {
            ShoppingCart shoppingCart = new()
            {
                Quantity = 1,
                Product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(p => p.Id == productId, includeProperties: "Category,CoverType")
            };

            return View(shoppingCart);
        }

        [HttpPost("{productId}")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Index(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCart.ApplicationUserId = claim.Value;
            
            ShoppingCart currentShoppingCart = await _unitOfWork.ShoppingCartRepository.GetFirstOrDefaultAsync(s => s.ApplicationUserId == claim.Value 
                && s.ProductId == shoppingCart.ProductId, includeProperties: "Product");

            if (currentShoppingCart == null)
            {
                var product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(p => p.Id == shoppingCart.ProductId);
                if (product != null)
                {
                    shoppingCart.Product = product;
                    _unitOfWork.ShoppingCartRepository.SetUnitPrice(shoppingCart);
                    _unitOfWork.ShoppingCartRepository.SetFinalPrice(shoppingCart);

                    await _unitOfWork.ShoppingCartRepository.AddAsync(shoppingCart);
                }
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.IncrementQuantity(currentShoppingCart, shoppingCart.Quantity);
                _unitOfWork.ShoppingCartRepository.SetUnitPrice(currentShoppingCart);
                _unitOfWork.ShoppingCartRepository.SetFinalPrice(currentShoppingCart);
            }

            await _unitOfWork.SaveChangesAsync();

            return RedirectToAction("Index", "Cart");
        }
    }
}

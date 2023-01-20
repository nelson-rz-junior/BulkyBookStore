using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBook.ViewComponents;

public class ShoppingCartViewComponent: ViewComponent
{
    private readonly IUnitOfWork _unitOfWork;

    public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
	{
		_unitOfWork = unitOfWork;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

		if (claim != null)
		{
			var totalItems = HttpContext.Session.GetInt32(SD.SESSION_CART);
			if (totalItems == null)
			{
                var items = await _unitOfWork.ShoppingCartRepository.GetAllAsync(s => s.ApplicationUserId == claim.Value, includeProperties: "Product");
                totalItems = items.Sum(i => i.Quantity);

                HttpContext.Session.SetInt32(SD.SESSION_CART, totalItems.GetValueOrDefault());
            }

			return View(totalItems.GetValueOrDefault());
        }
		else
		{
			HttpContext.Session.Remove(SD.SESSION_CART);
			return View(default(int));
		}
	}
}

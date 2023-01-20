using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.ROLE_USER_ADMIN)]
	public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Categories = _unitOfWork.CategoryRepository.GetCategoryListForDropDown(),
                CoverTypes = _unitOfWork.CoverTypeRepository.GetCoverTypeListForDropDown()
            };

            if (!id.HasValue || id == 0)
            {
                productVM.Product = new();
            }
            else
            {
                productVM.Product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(c => c.Id == id.Value);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
            }

            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM productVM, IFormFile? fileUpload)
        {
            if (ModelState.IsValid)
            {
                if (fileUpload != null)
                {
                    byte[] image = null;

                    using (var fs = fileUpload.OpenReadStream())
                    {
                        using var ms = new MemoryStream();

                        fs.CopyTo(ms);
                        image = ms.ToArray();
                    }

                    productVM.Product.Image = image;
                }
                else
                {
                    var currentProduct = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(p => p.Id == productVM.Product.Id);
                    productVM.Product.Image = currentProduct.Image;
                }

                bool isCreateProduct = productVM.Product.Id == 0;

                if (isCreateProduct)
                {
                    await _unitOfWork.ProductRepository.AddAsync(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepository.Update(productVM.Product);
                }

                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Product #{productVM.Product.Id} {(isCreateProduct ? "created" : "updated")} successfully"; 

                return RedirectToAction(nameof(Index));
            }

            return View(productVM);
        }

        #region API Calls

        [HttpGet("api/products")]
        public async Task<IActionResult> GetProductsDT()
        {
            return Json(new
            {
                data = await _unitOfWork.ProductRepository.GetProductsDT()
            });
        }

        [HttpDelete("api/[controller]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            if (product == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Product was not found"
                });
            }

            _unitOfWork.ProductRepository.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Product deleted successfully"
            });
        }

        #endregion
    }
}

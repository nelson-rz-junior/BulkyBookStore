using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (_unitOfWork.CategoryRepository.Exists(c => c.Name == category.Name))
            {
                ModelState.AddModelError("Name", "The category name already exists.");
            }

            if (_unitOfWork.CategoryRepository.Exists(c => c.DisplayOrder == category.DisplayOrder))
            {
                ModelState.AddModelError("DisplayOrder", "The display order already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Category #{category.Id} created successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            if (_unitOfWork.CategoryRepository.Exists(c => c.Id != category.Id && c.Name == category.Name))
            {
                ModelState.AddModelError("Name", "The category name already exists.");
            }

            if (_unitOfWork.CategoryRepository.Exists(c => c.Id != category.Id && c.DisplayOrder == category.DisplayOrder))
            {
                ModelState.AddModelError("DisplayOrder", "The display order already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Category #{category.Id} updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var category = await _unitOfWork.CategoryRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            _unitOfWork.CategoryRepository.Remove(category);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Category #{id} deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}

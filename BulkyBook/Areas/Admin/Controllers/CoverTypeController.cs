using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.ROLE_USER_ADMIN)]
	public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var coverTypes = await _unitOfWork.CoverTypeRepository.GetAllAsync();
            return View(coverTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CoverType coverType)
        {
            if (_unitOfWork.CoverTypeRepository.Exists(c => c.Name == coverType.Name))
            {
                ModelState.AddModelError("Name", "The cover type name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(coverType);
            }

            await _unitOfWork.CoverTypeRepository.AddAsync(coverType);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cover type #{coverType.Id} created successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var coverType = await _unitOfWork.CoverTypeRepository.GetFirstOrDefaultAsync(c => c.Id == id.Value);
            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CoverType coverType)
        {
            if (_unitOfWork.CoverTypeRepository.Exists(c => c.Id != coverType.Id && c.Name == coverType.Name))
            {
                ModelState.AddModelError("Name", "The cover type name already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(coverType);
            }

            _unitOfWork.CoverTypeRepository.Update(coverType);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cover type #{coverType.Id} updated successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue || id == 0)
            {
                return NotFound();
            }

            var coverType = await _unitOfWork.CoverTypeRepository.GetFirstOrDefaultAsync(c => c.Id == id.Value);
            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var coverType = await _unitOfWork.CoverTypeRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            if (coverType == null)
            {
                return NotFound();
            }

            _unitOfWork.CoverTypeRepository.Remove(coverType);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cover type #{id} deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}

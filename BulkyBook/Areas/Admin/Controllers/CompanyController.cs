using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.ROLE_USER_ADMIN)]
	public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
            return View(companies);
        }

        public async Task<IActionResult> Upsert(int? id)
        {
            Company company;

            if (!id.HasValue || id == 0)
            {
                company = new();
            }
            else
            {
                company = await _unitOfWork.CompanyRepository.GetFirstOrDefaultAsync(c => c.Id == id.Value);
                if (company == null)
                {
                    return NotFound();
                }
            }

            return View(company);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                bool isCreateCompany = company.Id == 0;

                if (isCreateCompany)
                {
                    await _unitOfWork.CompanyRepository.AddAsync(company);
                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(company);
                }

                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Company #{company.Id} {(isCreateCompany ? "created" : "updated")} successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(company);
        }

        #region API Calls

        [HttpGet("api/companies")]
        public async Task<IActionResult> GetCompaniesDT()
        {
            return Json(new
            {
                data = await _unitOfWork.CompanyRepository.GetAllAsync()
            });
        }

        [HttpDelete("api/[controller]/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _unitOfWork.CompanyRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Company was not found"
                });
            }

            _unitOfWork.CompanyRepository.Remove(company);
            await _unitOfWork.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Company deleted successfully"
            });
        }

        #endregion
    }
}

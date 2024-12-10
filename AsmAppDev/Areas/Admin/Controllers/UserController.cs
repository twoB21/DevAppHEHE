using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AsmAppDev.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;

		public UsersController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			var userList = _unitOfWork.AppUserRepository.GetAll().ToList();
			return View(userList);
		}

		public async Task<IActionResult> ToggleStatus(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var user = _unitOfWork.AppUserRepository.Get(u => u.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			// Update the status
			user.Status = !user.Status;

			// Persist changes
			_unitOfWork.Save();

			TempData["Success"] = user.Status
				? "Account enabled successfully!"
				: "Account disabled successfully!";

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Delete(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return NotFound();
			}

			var user = _unitOfWork.AppUserRepository.Get(u => u.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			return View(user);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public IActionResult DeleteConfirmed(string id)
		{
			var user = _unitOfWork.AppUserRepository.Get(u => u.Id == id);
			if (user == null)
			{
				return NotFound();
			}

			_unitOfWork.AppUserRepository.Delete(user);
			_unitOfWork.Save();

			TempData["Success"] = "Account deleted successfully!";
			return RedirectToAction(nameof(Index));
		}
	}

}

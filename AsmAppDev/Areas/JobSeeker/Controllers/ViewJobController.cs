using AsmAppDev.Models.ViewModels;
using AsmAppDev.Models;
using AsmAppDev.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AsmAppDev.Areas.JobSeeker.Controllers
{
	[Area("JobSeeker")]
	public class ViewJobController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;

		public ViewJobController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;
		}

		// Displays the list of jobs
		public IActionResult Index()
		{
			var jobList = _unitOfWork.JobRepository.GetAll("Category").ToList();
			return View(jobList);
		}

		// Display the job application page
		public IActionResult Apply(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var job = _unitOfWork.JobRepository.Get(j => j.Id == id);
			if (job == null)
			{
				return NotFound();
			}

			var jobViewModel = new JobVM
			{
				apply = new JobApplication { JobId = job.Id },
				Job = job
			};

			return View(jobViewModel);
		}

		// Handles job application submission
		[HttpPost]
		public async Task<IActionResult> Apply(JobVM jobViewModel)
		{
			if (ModelState.IsValid)
			{
				var currentUser = await _userManager.GetUserAsync(User);

				if (currentUser == null)
				{
					return RedirectToAction("Login", "Account");
				}

				// Set applicant details
				jobViewModel.apply.Email = currentUser.Email;
				jobViewModel.apply.DayApply = DateTime.Now;

				// Save application to the database
				_unitOfWork.JobApplicationRepository.Add(jobViewModel.apply);
				_unitOfWork.Save();

				TempData["success"] = "Job applied successfully!";
				return RedirectToAction("Index");
			}

			return View(jobViewModel);
		}
	}
}

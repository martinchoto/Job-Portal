﻿using JobPortal.Core.Data.Models;
using JobPortal.Services.Application;
using JobPortal.ViewModels.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobPortal.Controllers
{
	[Authorize(Roles = "Applicant,Company")]
	public class ApplicationController : Controller
	{
		private readonly IApplicationService _applicationService;
		public ApplicationController(IApplicationService applicationService)
		{
			_applicationService = applicationService;
		}
		[Authorize(Roles = "Applicant")]
		public async Task<IActionResult> Mine()
		{
			List<MyJobApplicationViewModel> result = 
				await _applicationService.GetApplicationsAsync(GetUserId());

			return View(result);
		}
		[HttpGet]
		[Authorize(Roles = "Applicant")]
		public async Task<IActionResult> Create()
		{
			AddJobApplicationViewModel viewModel = new AddJobApplicationViewModel();

			return View(viewModel);
		}
		[HttpPost]
		[Authorize(Roles = "Applicant")]
		public async Task<IActionResult> Create(AddJobApplicationViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(viewModel);
			}
			await _applicationService.AddApplicationAsync(viewModel, GetUserId());
			return RedirectToAction(nameof(Mine), "Application");
		}
		[HttpGet]
		[Authorize(Roles = "Applicant")]
		public async Task<IActionResult> Edit(int id)
		{
			var jobApplication = await _applicationService.GetApplication(id);
			if (jobApplication == null)
			{
				return BadRequest();
			}
			if (jobApplication.UserId != GetUserId())
			{
				return Unauthorized();
			}
			AddJobApplicationViewModel viewModel = await _applicationService.BuildViewModel(jobApplication);
			return View(viewModel);
		}
		[HttpPost]
		[Authorize(Roles = "Applicant")]
		public async Task<IActionResult> Edit(AddJobApplicationViewModel model, int id)
		{
			JobApplication jobApplication = await _applicationService.GetApplication(id);
			if (jobApplication == null)
			{
				return BadRequest();
			}
			if (jobApplication.UserId != GetUserId())
			{
				return Unauthorized();
			}
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			await _applicationService.EditJobApplicationAsync(model, id);
			return RedirectToAction(nameof(Mine), "Application");
		}
		[Authorize(Roles = "Applicant")]
		public async Task<IActionResult> Delete(int id)
		{
			JobApplication application = await _applicationService.GetApplication(id);
			if (application == null)
			{
				return BadRequest();
			}
			if (application.UserId != GetUserId())
			{
				return Unauthorized();
			}
			DeleteApplicationViewModel model = await _applicationService.BuildDeleteModelAsync(application);
			return View(model);
		}
		[Authorize(Roles = "Applicant")]
		public async Task<IActionResult> ConfirmDelete(int id)
		{
			JobApplication application = await _applicationService.GetApplication(id);
			if (application == null)
			{
				return BadRequest();
			}
			if (application.UserId != GetUserId())
			{
				return Unauthorized();
			}
			await _applicationService.DeleteApplicationAsync(application);
			return RedirectToAction(nameof(Mine), "Application");
		}
		public async Task<IActionResult> Details(int id, string info)
		{
			JobApplication jobApp = await _applicationService.GetApplication(id);
			if (jobApp == null)
			{
				return BadRequest();
			}
			if (jobApp.UserId != GetUserId() && jobApp.JobOfferApplications.FirstOrDefault(x => x.JobOffer.Company.UserId == GetUserId()) == null)
			{
				return Unauthorized();
			}
			DetailsApplicationViewModel model = await _applicationService.BuildDetailsViewModelAsync(jobApp);
			return View(model);
		}
		private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier);
	}
}

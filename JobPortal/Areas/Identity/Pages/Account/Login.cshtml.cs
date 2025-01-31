﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using JobPortal.Core.Data.Identity;

namespace JobPortal.Areas.Identity.Pages.Account
{
	public class LoginModel : PageModel
	{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly ILogger<LoginModel> _logger;

		public LoginModel(SignInManager<AppUser> signInManager, ILogger<LoginModel> logger)
		{
			_signInManager = signInManager;
			_logger = logger;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }
		public string ReturnUrl { get; set; }
		[TempData]
		public string ErrorMessage { get; set; }
		public class InputModel
		{
			[Required]
			public string Username { get; set; }

			[Required]
			[DataType(DataType.Password)]
			public string Password { get; set; }
			/*[Display(Name = "Remember me?")]
			public bool RememberMe { get; set; }*/
		}

		public async Task<IActionResult> OnGetAsync(string returnUrl = null)
		{
			if (!string.IsNullOrEmpty(ErrorMessage))
			{
				ModelState.AddModelError(string.Empty, ErrorMessage);
			}
			if (User.Identity.IsAuthenticated && User.Identity != null)
			{
				return RedirectToAction("Index", "Home");
			}
			returnUrl ??= Url.Content("~/");

			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			ReturnUrl = returnUrl;

			return Page();
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");

			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(Input.Username,
					Input.Password,
					true,
					lockoutOnFailure: false);
				if (result.Succeeded)
				{
					_logger.LogInformation("User logged in.");
					if (returnUrl != null)
					{
						return LocalRedirect(returnUrl);
					}
					else
					{
						return RedirectToAction("Index", "Home");	
					}
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return Page();
				}
			}

			return Page();
		}
	}
}

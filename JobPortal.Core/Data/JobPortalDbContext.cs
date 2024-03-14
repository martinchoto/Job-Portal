﻿using JobPortal.Core.Data.Identity;
using JobPortal.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Job_Portal.Data
{
	public class JobPortalDbContext : IdentityDbContext<AppUser>
	{

		public JobPortalDbContext(DbContextOptions<JobPortalDbContext> options)
			: base(options)
		{
		}
		public DbSet<JobOffer> JobOffers { get; set; } = null!;
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseLazyLoadingProxies(true);
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

		}
	}
}

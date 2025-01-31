﻿using JobPortal.Core;
using JobPortal.Core.Data.Identity;
using JobPortal.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Type = JobPortal.Core.Data.Models.Type;

namespace JobPortal.Core.Data
{
	public class JobPortalDbContext : IdentityDbContext<AppUser>
	{
		private readonly SeedData seedData;
		private bool _seedDb;
		public JobPortalDbContext(DbContextOptions<JobPortalDbContext> options, bool seedDb = true)
			: base(options)
		{

			if (Database.IsRelational())
			{
				Database.Migrate();
			}
			else
			{
				Database.EnsureCreated();
			}
			seedData = new SeedData();
			_seedDb = seedDb;
		}

		public DbSet<Company> Companies { get; set; } = null!;
		public DbSet<JobOffer> JobOffers { get; set; } = null!;
		public DbSet<Type> Types { get; set; } = null!;
		public DbSet<JobApplication> Applications { get; set; } = null!;
		public DbSet<JobOfferApplication> JobOffersApplications { get; set; } = null!;
		public DbSet<Event> Events { get; set; } = null!;
		public DbSet<EventParticipants> EventsParticipants { get; set; } = null!;
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);

			optionsBuilder.UseLazyLoadingProxies();
			optionsBuilder.EnableSensitiveDataLogging();
		}
		protected override void OnModelCreating(ModelBuilder builder)
		{
			SeedData(builder);

			base.OnModelCreating(builder);

			builder.Entity<JobOfferApplication>()
				.HasKey(pk => new { pk.JobOfferId, pk.ApplicationId });

			builder.Entity<JobOfferApplication>()
				.HasOne(jo => jo.JobOffer)
				.WithMany(jo => jo.JobOfferApplications)
				.HasForeignKey(fk => fk.JobOfferId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<JobOfferApplication>()
				.HasOne(a => a.Application)
				.WithMany(jo => jo.JobOfferApplications)
				.HasForeignKey(fk => fk.ApplicationId).
				OnDelete(DeleteBehavior.Restrict);

			builder.Entity<EventParticipants>()
				.HasKey(pk => new { pk.EventId, pk.ParticipantId });

			builder.Entity<EventParticipants>()
				.HasOne(e => e.Event)
				.WithMany(p => p.EventParticipants)
				.HasForeignKey(fk => fk.EventId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<EventParticipants>()
				.HasOne(p => p.Participant)
				.WithMany(e => e.EventParticipants)
				.HasForeignKey(fk => fk.ParticipantId).
				OnDelete(DeleteBehavior.Restrict);
		}
		private void SeedData(ModelBuilder builder)
		{
			if (_seedDb)
			{
				builder.Entity<AppUser>()
								.HasData(seedData.SeedUsers());
				builder.Entity<IdentityRole>()
					.HasData(seedData.SeedRoles());
				builder.Entity<IdentityUserRole<string>>()
					.HasData(seedData.SeedUserRoles());
				builder.Entity<Type>()
					.HasData(seedData.SeedTypes());
				builder.Entity<Company>()
					.HasData(seedData.SeedCompanies());
				builder.Entity<Event>()
					.HasData(seedData.SeedEvent());
				builder.Entity<JobApplication>()
					.HasData(seedData.SeedApplications());
				builder.Entity<JobOffer>()
					.HasData(seedData.SeedOffers());
			}

		}
	}
}

using Microsoft.EntityFrameworkCore;
using System;
using TransactionalService.Core.Enumerations;
using TransactionalService.EntityFramework.Core.Entities;
using TransactionalService.EntityFramework.Core.Interceptors;

namespace TransactionalService.EntityFramework.Core.DbContexts
{
	public partial class TransactionDbContext : DbContext
	{
		private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

		public TransactionDbContext(DbContextOptions<TransactionDbContext> options, AuditableEntityInterceptor auditableEntityInterceptor)
			: base(options)
		{
			_auditableEntityInterceptor = auditableEntityInterceptor;
		}

		public virtual DbSet<Plan> Plans { get; set; }

		public virtual DbSet<Invoice> Invoices { get; set; }

		public virtual DbSet<Payment> Payments { get; set; }

		public virtual DbSet<Subscription> Subscriptions { get; set; }

		//public virtual DbSet<Wallet> Wallets { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<Wallet>(entity =>
			//{
			//	entity.HasKey(e => e.Id).HasName("Wallet_pkey");

			//	entity.ToTable("Wallet");
			//	entity.Property(e => e.Balance).HasColumnType("decimal(18,2)").HasColumnName("balance");
			//	entity.Property(e => e.UserId).IsRequired(true).HasColumnName("userId"); ;
			//	entity.Property(e => e.Currency).HasMaxLength(10).HasColumnName("currency"); ;
			//	entity.Property(e => e.IsActive).HasDefaultValue(true).HasColumnName("isActive");
			//	entity.Property(e => e.CreatedAt)
			//	.HasDefaultValueSql("CURRENT_TIMESTAMP")
			//	.HasColumnType("timestamp without time zone")
			//	.HasColumnName("createdAt");

			//	entity.Property(e => e.UpdatedAt)
			//		.HasDefaultValueSql("CURRENT_TIMESTAMP")
			//		.HasColumnType("timestamp without time zone")
			//		.HasColumnName("updatedAt");
			//});

			// Payment
			modelBuilder.Entity<Payment>(entity =>
			{
				entity.HasKey(e => e.Id).HasName("Payment_pkey");

				entity.ToTable("Payment");
				entity.Property(e => e.UserId).IsRequired(true).HasColumnName("userId"); ;
				entity.Property(e => e.SubscriptionId).IsRequired(true).HasColumnName("subscriptionId"); ;
				entity.Property(e => e.PaymentMethod).HasMaxLength(255).HasColumnName("paymentMethod"); ;
				entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").HasColumnName("amount"); ;
				entity.Property(e => e.TransactionId).HasMaxLength(255).HasColumnName("transactionId"); ;
				entity.Property(e => e.Currency)
				.HasMaxLength(50)
				.HasColumnName("currency")
				.HasConversion(
					v => v.ToString(),
					v => (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), v));
				entity.Property(e => e.Status)
				.HasMaxLength(50)
				.HasColumnName("status")
				.HasConversion(
					v => v.ToString(),
					v => (PaymentStatusEnum)Enum.Parse(typeof(PaymentStatusEnum), v));
				entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");

				entity.Property(e => e.UpdatedAt)
					.HasDefaultValueSql("CURRENT_TIMESTAMP")
					.HasColumnType("timestamp without time zone")
					.HasColumnName("updatedAt");
				entity.HasOne(e => e.Subscription)
					  .WithMany(s => s.Payments)
					  .HasForeignKey(e => e.SubscriptionId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// Invoice
			modelBuilder.Entity<Invoice>(entity =>
			{
				entity.HasKey(e => e.Id).HasName("Invoice_pkey");

				entity.ToTable("Invoice");
				entity.Property(e => e.UserId).IsRequired(true).HasColumnName("userId"); ;
				entity.Property(e => e.SubscriptionId).IsRequired(true).HasColumnName("subscriptionId");
				entity.Property(e => e.PaymentId).IsRequired(true).HasColumnName("paymentId");
				entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").HasColumnName("totalAmount");
				entity.Property(e => e.Status)
				.HasMaxLength(50)
				.HasColumnName("status")
				.HasConversion(
					v => v.ToString(),
					v => (PaymentStatusEnum)Enum.Parse(typeof(PaymentStatusEnum), v));
				entity.Property(e => e.IssuedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("issuedAt");

				entity.Property(e => e.DueDate)
					.HasDefaultValueSql("CURRENT_TIMESTAMP")
					.HasColumnName("dueDate");
				entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");

				entity.Property(e => e.UpdatedAt)
					.HasDefaultValueSql("CURRENT_TIMESTAMP")
					.HasColumnType("timestamp without time zone")
					.HasColumnName("updatedAt");
				entity.HasOne(e => e.Payment)
					  .WithOne()
					  .HasForeignKey<Invoice>(e => e.PaymentId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			// Plan
			modelBuilder.Entity<Plan>(entity =>
			{
				entity.HasKey(e => e.Id).HasName("Plan_pkey");

				entity.ToTable("Plan");
				entity.Property(e => e.PlanPrice).HasColumnType("decimal(18,2)").HasColumnName("planPrice");
				entity.Property(e => e.PlanName).HasMaxLength(500).HasColumnName("planName");
				entity.Property(e => e.Duration).IsRequired().HasColumnName("duration");
				entity.Property(e => e.IsActive).HasColumnName("isActive");
				entity.Property(e => e.Currency)
				.HasMaxLength(50)
				.HasColumnName("currency")
				.HasConversion(
					v => v.ToString(),
					v => (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), v));
				entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");

				entity.Property(e => e.UpdatedAt)
					.HasDefaultValueSql("CURRENT_TIMESTAMP")
					.HasColumnType("timestamp without time zone")
					.HasColumnName("updatedAt");
			});

			// Subscription
			modelBuilder.Entity<Subscription>(entity =>
			{
				entity.HasKey(e => e.Id).HasName("Subscription_pkey");

				entity.ToTable("Subscription");
				entity.Property(e => e.UserId).IsRequired(true).HasColumnName("userId"); ;
				entity.Property(e => e.PlanId).HasColumnName("planId"); ;
				entity.Property(e => e.Status)
				.HasMaxLength(50)
				.HasColumnName("status")
				.HasConversion(
					v => v.ToString(),
					v => (SubscriptionEnum)Enum.Parse(typeof(SubscriptionEnum), v));
				entity.Property(e => e.StartDate)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnName("startDate");
				entity.Property(e => e.EndDate)
					.HasDefaultValueSql("CURRENT_TIMESTAMP")
					.HasColumnName("endDate");
				entity.Property(e => e.CreatedAt)
				.HasDefaultValueSql("CURRENT_TIMESTAMP")
				.HasColumnType("timestamp without time zone")
				.HasColumnName("createdAt");
				entity.Property(e => e.UpdatedAt)
					.HasDefaultValueSql("CURRENT_TIMESTAMP")
					.HasColumnType("timestamp without time zone")
					.HasColumnName("updatedAt");
				entity.HasOne(e => e.Plan)
					  .WithMany(p => p.Subscriptions)
					  .HasForeignKey(e => e.PlanId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			OnModelCreatingPartial(modelBuilder);
		}

		partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
	}
}
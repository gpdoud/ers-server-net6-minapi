using System;
using Microsoft.EntityFrameworkCore;

namespace ers_server_net6_minapi.Models {

    public class AppDbContext : DbContext {

        public DbSet<Expense>? Expenses { get; set; }
        public DbSet<Expenseline>? Expenselines { get; set; }
        public DbSet<Category>? Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<Expense>(e => {
                e.HasKey(p => p.Id);
                e.Property(p => p.Description)
                    .HasMaxLength(50)
                    .IsRequired();
                e.Property(p => p.Payee)
                    .HasMaxLength(30)
                    .IsRequired();
                e.Property(p => p.TotalExpenses)
                    .HasColumnType("decimal(9,2)");
                e.Property(p => p.TotalReimbursed)
                    .HasColumnType("decimal(9,2)");
                e.Property(p => p.Locked); // bool; cannot be changed
                e.Property(p => p.Active) // bool
                    .IsRequired(false);
                e.Property(p => p.Created) // datetime
                    .IsRequired(false);
                e.Property(p => p.Updated) // datetime
                    .IsRequired(false);
                e.HasMany(p => p.Expenselines)
                    .WithOne(p => p.Expense)
                    .HasForeignKey(p => p.ExpenseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            builder.Entity<Expenseline>(e => {
                e.HasKey(p => p.Id);
                e.Property(p => p.Description) // string 50
                    .HasMaxLength(50).IsRequired();
                e.Property(p => p.Amount) // decimal
                    .HasColumnType("decimal(9,2)");
                e.Property(p => p.Prepaid); // bool
                e.Property(p => p.Active) // bool
                    .IsRequired(false);
                e.Property(p => p.Created) // datetime
                    .IsRequired(false);
                e.Property(p => p.Updated) // datetime
                    .IsRequired(false);
                e.HasOne(p => p.Category) // FK
                    .WithMany(p => p.Expenselines)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(p => p.Expense) // FK
                    .WithMany(p => p.Expenselines)
                    .HasForeignKey(p => p.ExpenseId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.Ignore(p => p.Expense);
            });
            builder.Entity<Category>(e => {
                e.HasKey(p => p.Id);
                e.Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired();
                e.Property(p => p.MaxAmount) // decimal
                    .HasColumnType("decimal(9,2)");
                e.Property(p => p.Active) // bool
                    .IsRequired(false);
                e.Property(p => p.Created) // datetime
                    .IsRequired(false);
                e.Property(p => p.Updated) // datetime
                    .IsRequired(false);
                e.HasMany(p => p.Expenselines)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.Ignore(p => p.Expenselines);
            });
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    }
}



using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spice.Models;

namespace Spice.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Category> Category { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<MenuItem> MenuItem { get; set; }
        public DbSet<Coupon> Coupon { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<OrderHeader> OrderHeader { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<News> News { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category model
            modelBuilder.Entity<Category>().HasKey(a => a.Id);
            modelBuilder.Entity<Category>()
              .Property(a => a.Name)
              .IsRequired();

            //SubCategory model
            modelBuilder.Entity<SubCategory>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<SubCategory>()
                .HasOne(a => a.Category)
                .WithMany(b => b.SubCategories)
                .HasForeignKey(a => a.CategoryId);
            modelBuilder.Entity<SubCategory>()
                .Property(a => a.Name)
                .IsRequired();
            modelBuilder.Entity<SubCategory>()
                .Property(a => a.CategoryId)
                .IsRequired();

            //MenuItem
            modelBuilder.Entity<MenuItem>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<MenuItem>()
                .HasOne(a => a.SubCategory)
                .WithMany(b => b.MenuItems)
                .HasForeignKey(a => a.SubCategoryId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MenuItem>()
                .HasOne(a => a.Category)
                .WithMany(b => b.MenuItems)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<MenuItem>()
                .Property(a => a.Name)
                .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.Price)
               .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.Color)
               .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.IsPublish)
               .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.Quantity)
               .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.PublishedDate)
               .IsRequired();

            //Coupon 
            modelBuilder.Entity<Coupon>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Coupon>()
                .Property(a => a.Name)
                .IsRequired();
            modelBuilder.Entity<Coupon>()
                .Property(a => a.CouponType)
                .IsRequired();
            modelBuilder.Entity<Coupon>()
                .Property(a => a.Discount)
                .IsRequired();
            modelBuilder.Entity<Coupon>()
                .Property(a => a.MinimumAmount)
                .IsRequired();

            //OrderHeader
            modelBuilder.Entity<OrderHeader>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<OrderHeader>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(b => b.OrderHeaders)
                .HasForeignKey(a => a.UserId);
            modelBuilder.Entity<OrderHeader>()
                .Property(a => a.UserId)
                .IsRequired();
            modelBuilder.Entity<OrderHeader>()
                .Property(a => a.OrderDate)
                .IsRequired();
            modelBuilder.Entity<OrderHeader>()
                .Property(a => a.OrderTotalOriginal)
                .IsRequired();
            modelBuilder.Entity<OrderHeader>()
                .Property(a => a.OrderTotal)
                .IsRequired();

            //OrderDetails
            modelBuilder.Entity<OrderDetails>()
               .HasKey(a => a.Id);
            modelBuilder.Entity<OrderDetails>()
               .HasOne(a => a.OrderHeader)
               .WithMany(b => b.OrderDetails)
               .HasForeignKey(a => a.OrderId);
            modelBuilder.Entity<OrderDetails>()
               .HasOne(a => a.MenuItem)
               .WithMany(b => b.OrderDetails)
               .HasForeignKey(a => a.MenuItemId);
            modelBuilder.Entity<OrderDetails>()
               .Property(a => a.Price)
               .IsRequired();
            modelBuilder.Entity<OrderDetails>()
               .Property(a => a.OrderId)
               .IsRequired();
            modelBuilder.Entity<OrderDetails>()
               .Property(a => a.MenuItemId)
               .IsRequired();


            // News
            modelBuilder.Entity<News>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<News>()
                .Property(a => a.Header)
                .IsRequired();
            modelBuilder.Entity<News>()
                .Property(a => a.Content)
                .IsRequired();
            modelBuilder.Entity<News>()
                .Property(a => a.ImageHeader)
                .IsRequired();
            modelBuilder.Entity<News>()
                .Property(a => a.Alias)
                .IsRequired();
            modelBuilder.Entity<News>()
                .Property(a => a.PublishedDate)
                .IsRequired();
            modelBuilder.Entity<News>()
                .Property(a => a.Type)
                .IsRequired();
            modelBuilder.Entity<News>()
                .Property(a => a.ApplicationUserId)
                .IsRequired();
            modelBuilder.Entity<News>()
                .Property(a => a.MenuItemId)
                .IsRequired(false);

            modelBuilder.Entity<News>()
                .HasOne(a => a.MenuItem)
                .WithMany(b => b.News)
                .HasForeignKey(a => a.MenuItemId);
            modelBuilder.Entity<News>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(b => b.News)
                .HasForeignKey(a => a.ApplicationUserId);
        }
    }
}

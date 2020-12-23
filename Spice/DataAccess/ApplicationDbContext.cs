using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using Spice.Utility;

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
        public DbSet<ImportHistory> ImportHistories { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<FavoritedProduct> FavoritedProducts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category model
            modelBuilder.Entity<Category>().HasKey(a => a.Id);
            modelBuilder.Entity<Category>()
              .Property(a => a.Id)
              .ValueGeneratedOnAdd();
            modelBuilder.Entity<Category>()
              .Property(a => a.Name)
              .IsRequired();

            //SubCategory model
            modelBuilder.Entity<SubCategory>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<SubCategory>()
              .Property(a => a.Id)
              .ValueGeneratedOnAdd();
            modelBuilder.Entity<SubCategory>()
                .Property(a => a.Name)
                .IsRequired();

            //MenuItem
            modelBuilder.Entity<MenuItem>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<MenuItem>()
              .Property(a => a.Id)
              .ValueGeneratedOnAdd();
            modelBuilder.Entity<MenuItem>()
                .HasOne(a => a.SubCategory)
                .WithMany(b => b.MenuItems)
                .HasForeignKey(a => a.SubCategoryId);
            modelBuilder.Entity<MenuItem>()
                .HasOne(a => a.Category)
                .WithMany(b => b.MenuItems)
                .HasForeignKey(a => a.CategoryId);
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
               .Property(a => a.PublishedDate)
               .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.Band)
               .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.Crystal)
               .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.Size)
               .IsRequired();
            modelBuilder.Entity<MenuItem>()
               .Property(a => a.Thickness)
               .IsRequired();

            //Coupon 
            modelBuilder.Entity<Coupon>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Coupon>()
              .Property(a => a.Id)
              .ValueGeneratedOnAdd();
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
              .Property(a => a.Id)
              .ValueGeneratedOnAdd();
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
              .Property(a => a.Id)
              .ValueGeneratedOnAdd();
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
              .Property(a => a.Id)
              .ValueGeneratedOnAdd();
            modelBuilder.Entity<News>()
                .Property(a => a.Header)
                .IsRequired();
            modelBuilder.Entity<News>()
                .Property(a => a.Content)
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
                .HasOne(a => a.MenuItem)
                .WithMany(b => b.News)
                .HasForeignKey(a => a.MenuItemId);

            //modelBuilder.Entity<News>()
            //    .HasOne(a => a.ApplicationUser)
            //    .WithMany(b => b.News)
            //    .HasForeignKey(a => a.ApplicationUserId);

            //Import History
            modelBuilder.Entity<ImportHistory>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<ImportHistory>()
              .Property(a => a.Id)
              .ValueGeneratedOnAdd();
            modelBuilder.Entity<ImportHistory>()
               .HasOne(a => a.ApplicationUser)
               .WithMany(b => b.ImportHistories)
               .HasForeignKey(a => a.UserId);
            modelBuilder.Entity<ImportHistory>()
               .HasOne(a => a.SubCategory)
               .WithMany(b => b.ImportHistories)
               .HasForeignKey(a => a.SubCategoryId)
               .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ImportHistory>()
               .HasOne(a => a.MenuItem)
               .WithMany(b => b.ImportHistories)
               .HasForeignKey(a => a.MenuItemID);
            modelBuilder.Entity<ImportHistory>()
               .Property(a => a.Quantity)
               .IsRequired();

            modelBuilder.Entity<Rating>()
                .HasKey(a => a.Id);
            modelBuilder.Entity<Rating>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Rating>()
                .Property(a => a.Comment)
                .IsRequired();
            modelBuilder.Entity<Rating>()
                .Property(a => a.RatingStar)
                .IsRequired();
            modelBuilder.Entity<Rating>()
                .Property(a => a.PublishedDate)
                .IsRequired();
            modelBuilder.Entity<Rating>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(b => b.Ratings)
                .HasForeignKey(a => a.UserId);
            modelBuilder.Entity<Rating>()
                .HasOne(a => a.MenuItem)
                .WithMany(b => b.Ratings)
                .HasForeignKey(a => a.MenuItemId);

            modelBuilder.Entity<FavoritedProduct>()
                .HasKey(pc => new { pc.UserId, pc.ItemId });
            modelBuilder.Entity<FavoritedProduct>()
                .HasOne(a => a.ApplicationUser)
                .WithMany(b => b.FavoritedProducts)
                .HasForeignKey(a => a.UserId);
            modelBuilder.Entity<FavoritedProduct>()
                .HasOne(a => a.MenuItem)
                .WithMany(b => b.FavoritedProducts)
                .HasForeignKey(a => a.ItemId);

            modelBuilder.Entity<Category>().HasData
                        (
                            new Category { Id = 1, Name = "SmartWatch" },
                            new Category { Id = 2, Name = "Analog" }
                        );

            modelBuilder.Entity<SubCategory>().HasData
                        (
                            new SubCategory { Id = 1, Name = "Apple" },
                            new SubCategory { Id = 2, Name = "Casio" },
                            new SubCategory { Id = 3, Name = "Rolex" },
                            new SubCategory { Id = 4, Name = "Samsung"}
                        );

            modelBuilder.Entity<Coupon>().HasData
                        (
                            new Coupon { Id = 1, Name = "15OFF", CouponType = "0", Discount = 15, MinimumAmount = 75, IsActive = true }                            
                        );

            modelBuilder.Entity<News>().HasData
                        (
                            new News
                            {
                                Id = 1,
                                Header = "Sale on for everything 15OFF",
                                Content = "<p>In this summer, we have a coupon for everything for 15 % each deal which is larger than 50 $&nbsp;</p><p><img alt=" + "15 Off Images, Stock Photos &amp; Vectors | Shutterstock" + "src=" + "https://www.springhillcourt.com/wp-content/uploads/2018/01/15off.jpg" + "/></p>",
                                Alias = "Sale-Off-15OFF",
                                PublishedDate = DateTime.Now,
                                Type = "1",
                                ImageHeader = "\\images\\News1.png"
                            }
                        );

            modelBuilder.Entity<MenuItem>().HasData
            (
                    new MenuItem { Id = 1, Name = "Rolex 1", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\1.png", Price = 100, IsPublish = true, Quantity = 6, Color = "3", Tag = "2", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 3 },
                    new MenuItem { Id = 2, Name = "Rolex 2", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\2.png", Price = 156, IsPublish = true, Quantity = 20, Color = "3", Tag = "2", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 3 },
                    new MenuItem { Id = 3, Name = "Rolex 3", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\3.png", Price = 25, IsPublish = true, Quantity = 23, Color = "3", Tag = "2", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 3 },
                    new MenuItem { Id = 4, Name = "Casio 1", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\4.png", Price = 245, IsPublish = true, Quantity = 20, Color = "1", Tag = "2", PublishedDate = DateTime.Now, CategoryId = 1, SubCategoryId = 2 },
                    new MenuItem { Id = 5, Name = "Casio 2", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\5.png", Price = 154, IsPublish = true, Quantity = 25, Color = "1", Tag = "1", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 2 },
                    new MenuItem { Id = 6, Name = "Casio 3", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\6.png", Price = 157, IsPublish = true, Quantity = 15, Color = "1", Tag = "1", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 2 },
                    new MenuItem { Id = 7, Name = "Samsung 1", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\7.png", Price = 198, IsPublish = true, Quantity = 23, Color = "3", Tag = "0", PublishedDate = DateTime.Now, CategoryId = 1, SubCategoryId = 4 },
                    new MenuItem { Id = 8, Name = "Apple 1", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\8.png", Price = 998, IsPublish = true, Quantity = 18, Color = "1", Tag = "0", PublishedDate = DateTime.Now, CategoryId = 1, SubCategoryId = 1 }
            );
        }
    }
}

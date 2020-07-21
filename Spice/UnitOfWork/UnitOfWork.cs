using Spice.Data;
using Spice.Models;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private ApplicationDbContext _context;
        private ICategoryRepository categoryRepository;
        private ISubCategoryRepository subCategoryRepository;
        private ICouponRepository couponRepository;
        private IImportHistoryRepository importHistoryRepository;
        private IApplicationUserRepository applicationUserRepository;
        private INewsRepository newsRepository;
        private IMenuItemRepository menuItemRepository;
        private bool disposed = false;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kiểm tra xem repository đã được khởi tạo chưa
        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new CategoryRepository(_context);
                }
                return this.categoryRepository;
            }
        }
        public ISubCategoryRepository SubCategoryRepository
        {
            get
            {
                if (this.subCategoryRepository == null)
                {
                    this.subCategoryRepository = new SubCategoryRepository(_context);
                }
                return this.subCategoryRepository;
            }
        }
        public ICouponRepository CouponRepository
        {
            get
            {
                if (this.couponRepository == null)
                {
                    this.couponRepository = new CouponRepository(_context);
                }
                return this.couponRepository;
            }
        }

        public IImportHistoryRepository ImportHistoryRepository 
        {
            get
            {
                if (this.importHistoryRepository == null)
                {
                    this.importHistoryRepository = new ImportHistoryRepository(_context);
                }
                return this.importHistoryRepository;
            }
        }

        public IApplicationUserRepository ApplicationUserRepository
        {
            get
            {
                if (this.applicationUserRepository == null)
                {
                    this.applicationUserRepository = new ApplicationUserRepository(_context);
                }
                return this.applicationUserRepository;
            }
        }
        public INewsRepository NewsRepository
        {
            get
            {
                if (this.newsRepository == null)
                {
                    this.newsRepository = new NewsRepository(_context);
                }
                return this.newsRepository;
            }
        }

        public IMenuItemRepository MenuItemRepository
        {
            get
            {
                if (this.menuItemRepository == null)
                {
                    this.menuItemRepository = new MenuItemRepository(_context);
                }
                return this.menuItemRepository;
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}

using Spice.Data;
using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public class UnitOfWork : IDisposable
    {
        private ApplicationDbContext _context;
        private GenericRepository<Category> departmentRepository;
        //private GenericRepository<Course> courseRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        // Kiểm tra xem repository đã được khởi tạo chưa
        public GenericRepository<Category> DepartmentRepository
        {
            get
            {
                if (this.departmentRepository == null)
                {
                    this.departmentRepository = new GenericRepository<Category>(_context);
                }
                return departmentRepository;
            }
        }

        //// Kiểm tra xem repository đã được khởi tạo chưa
        //public GenericRepository<Course> CourseRepository
        //{
        //    get
        //    {
        //        if (this.courseRepository == null)
        //        {
        //            this.courseRepository = new GenericRepository<Course>(context);
        //        }
        //        return courseRepository;
        //    }
        //}

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

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

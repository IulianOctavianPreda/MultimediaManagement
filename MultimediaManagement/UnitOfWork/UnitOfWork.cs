
using Microsoft.Win32.SafeHandles;
using MultimediaManagement.Models;
using MultimediaManagement.Repository;
using System;
using System.Runtime.InteropServices;

namespace MultimediaManagement.UoW
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        private MultimediaManagementContext _context;
        public IUserRepository _user;
        public ICollectionRepository _collection;
        public IPlaceholderRepository _placeholder;
        public IEntityFileRepository _entityFile;


        public IUserRepository User { get => _user; }
        public ICollectionRepository Collection { get => _collection; }
        public IPlaceholderRepository Placeholder { get => _placeholder; }
        public IEntityFileRepository EntityFile { get => _entityFile; }


        public UnitOfWork(MultimediaManagementContext context)
        {
            _context = context;
        }

        public void Create()
        {
            _user = new UserRepository(_context);
        }

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                _context.Dispose();
                // Free any other managed objects here.
            }
            disposed = true;
        }
    }
}

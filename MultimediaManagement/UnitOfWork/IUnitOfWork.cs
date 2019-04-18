
using MultimediaManagement.Repository;
using System;

namespace MultimediaManagement.UoW
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void Create();

        IUserRepository User { get;}
        ICollectionRepository Collection { get; }
        IPlaceholderRepository Placeholder { get; }
        IEntityFileRepository EntityFile { get; }
    }
}


using MultimediaManagement.Models;
using System.Collections.Generic;

namespace MultimediaManagement.Repository
{
    public class UserRepository : TemplateRepository<User>, IUserRepository
    {
        public UserRepository(MultimediaManagementContext context) : base(context)
        {
            
        }

        private bool _disposed = false;

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
            base.Dispose();
        }
    }
}

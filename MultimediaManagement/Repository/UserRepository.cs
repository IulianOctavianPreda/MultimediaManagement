
using MultimediaManagement.Models;
using System.Collections.Generic;

namespace MultimediaManagement.Repository
{
    public class UserRepository : TemplateRepository<User>, IUserRepository
    {
        public UserRepository(MultimediaManagementContext context) : base(context)
        {
            
        }
    }
}

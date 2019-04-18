using MultimediaManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaManagement.Repository
{
    public class PlaceholderRepository : TemplateRepository<Placeholder>, IPlaceholderRepository
    {
        public PlaceholderRepository(MultimediaManagementContext context) : base(context)
        {

        }
    }
}

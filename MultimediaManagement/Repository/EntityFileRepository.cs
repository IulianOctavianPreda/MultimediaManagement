using MultimediaManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultimediaManagement.Repository
{
    public class EntityFileRepository : TemplateRepository<EntityFile>, IEntityFileRepository
    {
        public EntityFileRepository(MultimediaManagementContext context) : base(context)
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

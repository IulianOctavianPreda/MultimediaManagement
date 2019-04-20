using MultimediaManagement.Models;


namespace MultimediaManagement.Repository
{
    public class CollectionRepository : TemplateRepository<Collection>, ICollectionRepository
    {
        public CollectionRepository(MultimediaManagementContext context) : base(context)
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

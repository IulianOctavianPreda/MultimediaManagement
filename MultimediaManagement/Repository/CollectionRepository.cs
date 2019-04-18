using MultimediaManagement.Models;


namespace MultimediaManagement.Repository
{
    public class CollectionRepository : TemplateRepository<Collection>, ICollectionRepository
    {
        public CollectionRepository(MultimediaManagementContext context) : base(context)
        {

        }
    }
}

using System;
using System.Collections.Generic;

namespace MultimediaManagement.Models
{
    public partial class Placeholder
    {
        public Placeholder()
        {
            EntityFile = new HashSet<EntityFile>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Keywords { get; set; }
        public Guid CollectionId { get; set; }
        public string Data { get; set; }
        public string Extension { get; set; }

        public virtual Collection Collection { get; set; }
        public virtual ICollection<EntityFile> EntityFile { get; set; }
    }
}

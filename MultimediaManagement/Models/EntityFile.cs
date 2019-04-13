using System;
using System.Collections.Generic;

namespace MultimediaManagement.Models
{
    public partial class EntityFile
    {
        public Guid Id { get; set; }
        public string Extension { get; set; }
        public Guid PlaceholderId { get; set; }
        public string Data { get; set; }
        public bool IsUrl { get; set; }

        public virtual Placeholder Placeholder { get; set; }
    }
}

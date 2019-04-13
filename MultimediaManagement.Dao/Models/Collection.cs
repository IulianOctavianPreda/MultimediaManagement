using System;
using System.Collections.Generic;

namespace MultimediaManagement.Dao.Models
{
    public partial class Collection
    {
        public Collection()
        {
            Placeholder = new HashSet<Placeholder>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte Type { get; set; }
        public string Keywords { get; set; }
        public Guid UserId { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Placeholder> Placeholder { get; set; }
    }
}

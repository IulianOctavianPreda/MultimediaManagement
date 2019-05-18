using System;
using System.Collections.Generic;

namespace MultimediaManagement.Models
{
    public partial class User
    {
        public User()
        {
            Collection = new HashSet<Collection>();
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid Token { get; set; }
        public DateTime ModifiedOn { get; set; }



        public virtual ICollection<Collection> Collection { get; set; }
    }
}

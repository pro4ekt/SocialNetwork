using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    public class ClientProfile
    {
        [Key]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public string Address { get; set; }
        public int Age { get; set; }
        public string Info { get; set; }
        public bool Banned { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual List<Friends> Friends { get; set; }

        public ClientProfile()
        {
            Friends = new List<Friends>();
        }
    }
}

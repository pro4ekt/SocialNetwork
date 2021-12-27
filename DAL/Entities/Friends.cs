using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Friends
    {
        [Key,Column(Order = 0), ForeignKey("ClientProfile")]
        public string Id { get; set; }

        [Key,Column(Order = 1)]
        public string FriendId { get; set; }

        public virtual ClientProfile ClientProfile { get; set; }
    }
}

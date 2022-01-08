using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Messages
    {
        [Key, Column(Order = 0), ForeignKey("ClientProfile")]
        public string Id { get; set; }
        [Key, Column(Order = 1)]
        public string ReceiverId { get; set; }
        [Key, Column(Order = 2)]
        public string Text { get; set; }
        [Key, Column(Order = 3)]
        public DateTime DateTime { get; set; }
        public virtual ClientProfile ClientProfile { get; set; }
    }
}

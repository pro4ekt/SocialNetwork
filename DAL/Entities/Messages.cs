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
        [Key]
        public string MessageId { get; set; }

        [ForeignKey("ClientProfile")]
        public string Id { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }

        public virtual ClientProfile ClientProfile { get; set; }
    }
}

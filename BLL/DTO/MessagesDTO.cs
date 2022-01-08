using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class MessagesDTO
    {
        public string Id { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}

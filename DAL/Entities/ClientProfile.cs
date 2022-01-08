using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Entities
{
    /// <summary>
    /// Часть профиля пользователя в которой хранится информация про него (таблица формирует свезять один ко многим с Friends)
    /// </summary>
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
        public virtual List<Messages> Messages { get; set; }

        public ClientProfile()
        {
            Friends = new List<Friends>();
            Messages = new List<Messages>();
        }
    }
}

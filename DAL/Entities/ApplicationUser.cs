using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DAL.Entities
{
    /// <summary>
    /// Часть профиля пользователя на основе Identity (часть в которой хранится E-Mail UserName и все подтверждения)
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public virtual ClientProfile ClientProfile { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    /// <summary>
    /// DTO (Data Transfer Object) по сути нужен чтобы передавать данные из DAL в PL
    /// </summary>
    public class FriendsDTO
    {
        public string Id { get; set; }
        public string FriendId { get; set; }
    }
}

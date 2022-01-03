using System.Collections.Generic;

namespace BLL.DTO
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string Info { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public bool Banned { get; set; }
        public List<FriendsDTO> Friends { get; set; }

        public UserDTO()
        {
            Banned = false;
            Friends = new List<FriendsDTO>();
        }
    }
}

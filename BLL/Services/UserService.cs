using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Infrastructure;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNet.Identity;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> Create(UserDTO userDto)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            if (user == null)
            {
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.UserName };
                var result = Database.UserManager.Create(user, userDto.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                // добавляем роль
                await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);
                // создаем профиль клиента
                ClientProfile clientProfile = new ClientProfile { Id = user.Id, Address = userDto.Address, Age = userDto.Age, Info = userDto.Info};
                Database.ClientManager.Create(clientProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Register succesfull", "");
            }
            else
            {
                return new OperationDetails(false, "User with this login already exists", "Email");
            }
        }

        public async Task<bool> RemoveUser(string id)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
            if (user == null)
                return false;
            ClientProfile clientProfile = user.ClientProfile;
            Database.ClientManager.Remove(clientProfile);
            Database.UserManager.Delete(user);
            var allFriendships = Database.FriendsManager.GetAll();
            var friendshipsToRemove = allFriendships.Where(f=>(f.Id == user.Id || f.FriendId == user.Id));
            foreach (var f in friendshipsToRemove)
            {
                Database.FriendsManager.Remove(f);
            }
            await Database.SaveAsync();
            return true;
        }

        public async Task<bool> BanUser(string id)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
            UserDTO userDto = await FindById(id);
            if (user.ClientProfile.Banned == true)
                return false;
            user.ClientProfile.Banned = true;
            userDto.Banned = true;
            await Database.SaveAsync();
            return true;
        }

        public async Task<bool> UnBanUser(string id)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
            UserDTO userDto = await FindById(id);
            if (user.ClientProfile.Banned == false)
                return false;
            user.ClientProfile.Banned = false;
            userDto.Banned = false;
            await Database.SaveAsync();
            return true;
        }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
            // авторизуем его и возвращаем объект ClaimsIdentity
            if (user != null)
                claim = await Database.UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
            return claim;
        }

        public async Task<UserDTO> FindByEmail(string email)
        {
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(email);
            UserDTO userDto = new UserDTO();
            if (user != null)
            {
                List<string> a = new List<string>();
                foreach (var r in user.Roles.ToList())
                {
                    a.Add(r.RoleId);
                }
                userDto.Info = user.ClientProfile.Info;
                userDto.Address = user.ClientProfile.Address;
                userDto.Age = user.ClientProfile.Age;
                userDto.UserName = user.UserName;
                userDto.Id = user.Id;
                userDto.Email = user.Email;
                userDto.Password = user.PasswordHash;
                userDto.Role = Database.RoleManager.FindByIdAsync(a[0]).Result.Name;
                userDto.Banned = user.ClientProfile.Banned;
                foreach (var f in user.ClientProfile.Friends)
                {
                    userDto.Friends.Add(new FriendsDTO { Id = f.Id, FriendId = f.FriendId });
                }
                foreach (var m in user.ClientProfile.Messages)
                {
                    var u = await Database.UserManager.FindByIdAsync(m.ReceiverId);
                    if (u == null)
                        userDto.Messages.Add(new MessagesDTO { Id = m.Id, ReceiverId = m.ReceiverId, ReceiverName = "All", Text = m.Text, DateTime = m.DateTime });
                    else
                        userDto.Messages.Add(new MessagesDTO { Id = m.Id, ReceiverId = m.ReceiverId, ReceiverName = u.UserName, Text = m.Text, DateTime = m.DateTime });
                }
                return userDto;
            }
            return null;
        }
        public async Task<UserDTO> FindById(string id)
        {
            ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
            UserDTO userDto = new UserDTO();
            if (user != null)
            {
                List<string> a = new List<string>();
                foreach (var r in user.Roles.ToList())
                {
                    a.Add(r.RoleId);
                }
                userDto.Info = user.ClientProfile.Info;
                userDto.Address = user.ClientProfile.Address;
                userDto.Age = user.ClientProfile.Age;
                userDto.UserName = user.UserName;
                userDto.Email = user.Email;
                userDto.Id = user.Id;
                userDto.Password = user.PasswordHash;
                userDto.Role = Database.RoleManager.FindByIdAsync(a[0]).Result.Name;
                userDto.Banned = user.ClientProfile.Banned;
                foreach (var f in user.ClientProfile.Friends)
                {
                    userDto.Friends.Add(new FriendsDTO{Id = f.Id, FriendId = f.FriendId});
                }
                foreach (var m in user.ClientProfile.Messages)
                {
                    var u = await Database.UserManager.FindByIdAsync(m.ReceiverId);
                    if (u == null)
                        userDto.Messages.Add(new MessagesDTO { Id = m.Id, ReceiverId = m.ReceiverId, ReceiverName = "All", Text = m.Text, DateTime = m.DateTime });
                    else
                        userDto.Messages.Add(new MessagesDTO { Id = m.Id, ReceiverId = m.ReceiverId, ReceiverName = u.UserName, Text = m.Text, DateTime = m.DateTime });
                }
                return userDto;
            }
            return null;
        }
        public async Task<UserDTO> FindByName(string userName)
        {
            ApplicationUser user = await Database.UserManager.FindByNameAsync(userName);
            UserDTO userDto = new UserDTO();
            if (user != null)
            {
                List<string> a = new List<string>();
                foreach (var r in user.Roles.ToList())
                {
                    a.Add(r.RoleId);
                }

                userDto.Info = user.ClientProfile.Info;
                userDto.Address = user.ClientProfile.Address;
                userDto.Age = user.ClientProfile.Age;
                userDto.UserName = user.UserName;
                userDto.Id = user.Id;
                userDto.Email = user.Email;
                userDto.Password = user.PasswordHash;
                userDto.Role = Database.RoleManager.FindByIdAsync(a[0]).Result.Name;
                userDto.Banned = user.ClientProfile.Banned;
                foreach (var f in user.ClientProfile.Friends)
                {
                    userDto.Friends.Add(new FriendsDTO {Id = f.Id, FriendId = f.FriendId});
                }
                foreach (var m in user.ClientProfile.Messages)
                {
                    var u = await Database.UserManager.FindByIdAsync(m.ReceiverId);
                    if (u == null)
                        userDto.Messages.Add(new MessagesDTO { Id = m.Id, ReceiverId = m.ReceiverId, ReceiverName = "All", Text = m.Text, DateTime = m.DateTime });
                    else
                        userDto.Messages.Add(new MessagesDTO { Id = m.Id, ReceiverId = m.ReceiverId, ReceiverName = u.UserName, Text = m.Text, DateTime = m.DateTime });
                }
                return userDto;
            }
            return null;
        }

        public async Task<bool> EditProfile(string id,string name, string email, string info, string address,int age, UserDTO user)
        {
            try
            {
                if (Database.UserManager.FindByEmailAsync(email) != null && user.Email != email)
                    throw new Exception();
                Database.UserManager.FindByIdAsync(id).Result.ClientProfile.Address = address;
                Database.UserManager.FindByIdAsync(id).Result.Email = email;
                Database.UserManager.FindByIdAsync(id).Result.ClientProfile.Age = age;
                Database.UserManager.FindByIdAsync(id).Result.UserName = name;
                Database.UserManager.FindByIdAsync(id).Result.ClientProfile.Info = info;
                user.Email = email;
                user.Info = info;
                user.Address = address;
                user.Age = age;
                user.UserName = name;
                await Database.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddFriend(string id, string friendId)
        {
            try
            {
                if (id == friendId)
                    throw new Exception();
                Friends f1 = new Friends
                {
                    Id = id,
                    FriendId = friendId,
                    //ClientProfile = await Database.ClientManager.Find(id)
                };
                Database.FriendsManager.Create(f1);
                //Database.UserManager.FindByIdAsync(friendId).Result.ClientProfile.Friends.Add(f1);
                await Database.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> RemoveFriend(string id, string friendId)
        {
            try
            {
                Friends f1 = new Friends
                {
                    Id = id,
                    FriendId = friendId,
                    ClientProfile = await Database.ClientManager.Find(id)
                };
                Database.FriendsManager.Remove(f1);
                await Database.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckPassword(string email, string password)
        {
            PasswordHasher hasher = new PasswordHasher();
            ApplicationUser user = await Database.UserManager.FindByEmailAsync(email);
            if (hasher.VerifyHashedPassword(user.PasswordHash, password)
                != PasswordVerificationResult.Failed)
            {
                return true;
            }
            return false;
        }

        public async Task SaveMessage(MessagesDTO messageDto)
        {
            Messages message = new Messages
            {
                Id = messageDto.Id,
                ReceiverId = messageDto.ReceiverId,
                DateTime = messageDto.DateTime,
                Text = messageDto.Text,
            };
            Database.MessageManager.Create(message);
            await Database.SaveAsync();
        }

        public async Task RemoveMessage(string id)
        {

        }

        // начальная инициализация бд
        public async Task SetInitialData(List<UserDTO> usersDto, List<string> roles)
        {
            foreach (string roleName in roles)
            {
                var role = await Database.RoleManager.FindByNameAsync(roleName);
                if (role == null)
                {
                    role = new ApplicationRole { Name = roleName };
                    await Database.RoleManager.CreateAsync(role);
                }
            }

            foreach (var userDto in usersDto)
            {
                await Create(userDto);
            }
        }

        public void Dispose()
        {
            //ПРИДУМАТЬ ШОТО С ДИСПОЗОМ
        }
    }
}

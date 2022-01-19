using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
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
        private IUnitOfWork Database { get; set; }

        public UserService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<OperationDetails> Create(UserDTO userDto)
        {
            try
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
                    ClientProfile clientProfile = new ClientProfile { Id = user.Id, Address = userDto.Address, Age = userDto.Age, Info = userDto.Info };
                    Database.ClientManager.Create(clientProfile);
                    await Database.SaveAsync();
                    return new OperationDetails(true, "Register succesfull", "");
                }
                else
                {
                    return new OperationDetails(false, "User with this login already exists", "Email");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in Create");
                Console.WriteLine(e);
                return new OperationDetails(false, "User with this login already exists", "Email");
            }
        }
        public async Task<bool> RemoveUser(string id)
        {
            try
            {
                ApplicationUser user = await Database.UserManager.FindByIdAsync(id);
                if (user == null)
                    return false;
                ClientProfile clientProfile = user.ClientProfile;
                Database.ClientManager.Remove(clientProfile);
                Database.UserManager.Delete(user);
                var allFriendships = Database.FriendsManager.GetAll();
                var friendshipsToRemove = allFriendships.Where(f => (f.Id == user.Id || f.FriendId == user.Id));
                foreach (var f in friendshipsToRemove)
                {
                    Database.FriendsManager.Remove(f);
                }
                await Database.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in RemoveUser");
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> BanUser(string id)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in BanUser");
                Console.WriteLine(e);
                return false;
            }
        }
        public async Task<bool> UnBanUser(string id)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in UnBanUser");
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            try
            {
                // находим пользователя
                // авторизуем его и возвращаем объект ClaimsIdentity
                ClaimsIdentity claim = null;
                if (userDto != null)
                {
                    ApplicationUser user = await Database.UserManager.FindByEmailAsync(userDto.Email);
                    claim = await Database.UserManager.CreateIdentityAsync(user,
                        DefaultAuthenticationTypes.ApplicationCookie);
                    return claim;
                }
                return claim;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in Authenticate");
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<UserDTO> FindById(string id)
        {
            try
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
                        userDto.Friends.Add(new FriendsDTO { Id = f.Id, FriendId = f.FriendId });
                    }
                    foreach (var m in user.ClientProfile.Messages)
                    {
                        var u = await Database.UserManager.FindByIdAsync(m.ReceiverId);
                        if (u == null)
                            userDto.Messages.Add(new MessagesDTO { MessageId = m.MessageId, Id = m.Id, ReceiverId = m.ReceiverId, ReceiverName = "All", Text = m.Text, DateTime = m.DateTime });
                        else
                            userDto.Messages.Add(new MessagesDTO { MessageId = m.MessageId, Id = m.Id, ReceiverId = m.ReceiverId, ReceiverName = u.UserName, Text = m.Text, DateTime = m.DateTime });
                    }
                    return userDto;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in FindById");
                Console.WriteLine(e);
                return null;
            }
        }
        public async Task<UserDTO> FindByEmail(string email)
        {
            try
            {
                ApplicationUser user = await Database.UserManager.FindByEmailAsync(email);
                if (user != null)
                    return await FindById(user.Id);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in FindByEmail");
                Console.WriteLine(e);
                return null;
            }
        }
        public async Task<UserDTO> FindByName(string userName)
        {
            try
            {
                ApplicationUser user = await Database.UserManager.FindByNameAsync(userName);
                if (user != null)
                    return await FindById(user.Id);
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in FindByName");
                Console.WriteLine(e);
                return null;
            }
        }
        public async Task<List<UserDTO>> FindByAge(int age)
        {
            try
            {
                List<UserDTO> users = await FindAll();
                var ageUsers = users.Where(c => c.Age == age).ToList();
                return ageUsers;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in FindByAge");
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<UserDTO>> FindAll()
        {
            try
            {
                List<ApplicationUser> aU = Database.UserManager.Users.ToList();
                List<UserDTO> users = new List<UserDTO>();
                foreach (var user in aU)
                {
                    UserDTO userDto = await FindById(user.Id);
                    users.Add(userDto);
                }
                return users;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in FindAll");
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<bool> EditProfile(string id,string name, string email, string info, string address,int age, UserDTO user)
        {
            try
            {
                ApplicationUser u = await Database.UserManager.FindByEmailAsync(email);
                if (u != null && user.Email != email)
                    throw new Exception();
                else
                {
                    ApplicationUser u1 = await Database.UserManager.FindByEmailAsync(user.Email);
                    u1.Email = email;
                    u1.ClientProfile.Address = address;
                    u1.ClientProfile.Age = age;
                    u1.UserName = name;
                    u1.ClientProfile.Info = info;
                    user.Email = email;
                    user.Info = info;
                    user.Address = address;
                    user.Age = age;
                    user.UserName = name;
                    await Database.SaveAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in EditProfile");
                Console.WriteLine(e);
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
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in AddFriend");
                Console.WriteLine(e);
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
                    ClientProfile = Database.ClientManager.Find(id)
                };
                Database.FriendsManager.Remove(f1);
                await Database.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in RemoveFriend");
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> CheckPassword(string email, string password)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in CheckPassword");
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<bool> SaveMessage(MessagesDTO messageDto)
        {
            try
            {
                Messages message = new Messages
                {
                    MessageId = messageDto.MessageId,
                    Id = messageDto.Id,
                    ReceiverId = messageDto.ReceiverId,
                    DateTime = messageDto.DateTime,
                    Text = messageDto.Text,
                };
                Database.MessageManager.Create(message);
                await Database.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in SaveMessage");
                Console.WriteLine(e);
                return false;
            }
        }
        public async Task<bool> RemoveMessage(MessagesDTO messageDto)
        {
            try
            {
                Messages message = new Messages
                {
                    MessageId = messageDto.MessageId,
                    Id = messageDto.Id,
                    ReceiverId = messageDto.ReceiverId,
                    DateTime = messageDto.DateTime,
                    Text = messageDto.Text
                };
                Database.MessageManager.Remove(message);
                await Database.SaveAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in RemoveMessage");
                Console.WriteLine(e);
                return false;
            }
        }

        public async Task<List<MessagesDTO>> GetAllMessages()
        {
            try
            {
                List<Messages> mL = Database.MessageManager.GetAll();
                List<MessagesDTO> messages = new List<MessagesDTO>();
                foreach (var message in mL)
                {
                    var u = await Database.UserManager.FindByIdAsync(message.ReceiverId);
                    MessagesDTO messagesDto = new MessagesDTO
                    {
                        MessageId = message.MessageId,
                        Id = message.Id,
                        ReceiverId = message.ReceiverId,
                        ReceiverName = u.UserName,
                        Text = message.Text,
                        DateTime = message.DateTime
                    };
                    messages.Add(messagesDto);
                }
                return messages;
            }
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in GetAllMessages");
                Console.WriteLine(e);
                return null;
            }
        }

        // начальная инициализация бд
        public async Task SetInitialData(List<UserDTO> usersDto, List<string> roles)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(DateTime.Now + "Exception in SetInitialData");
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}

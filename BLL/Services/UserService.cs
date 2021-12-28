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
                user = new ApplicationUser { Email = userDto.Email, UserName = userDto.Email };
                var result = Database.UserManager.Create(user, userDto.Password);
                if (result.Errors.Count() > 0)
                    return new OperationDetails(false, result.Errors.FirstOrDefault(), "");
                // добавляем роль
                await Database.UserManager.AddToRoleAsync(user.Id, userDto.Role);
                // создаем профиль клиента
                ClientProfile clientProfile = new ClientProfile { Id = user.Id, Address = userDto.Address, Name = userDto.Name, Age = userDto.Age, Info = userDto.Info};
                Database.ClientManager.Create(clientProfile);
                await Database.SaveAsync();
                return new OperationDetails(true, "Регистрация успешно пройдена", "");
            }
            else
            {
                return new OperationDetails(false, "Пользователь с таким логином уже существует", "Email");
            }
        }

        public async Task<ClaimsIdentity> Authenticate(UserDTO userDto)
        {
            ClaimsIdentity claim = null;
            // находим пользователя
            ApplicationUser user = await Database.UserManager.FindAsync(userDto.Email, userDto.Password);
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
                userDto.Info = user.ClientProfile.Info;
                userDto.Address = user.ClientProfile.Address;
                userDto.Age = user.ClientProfile.Age;
                userDto.UserName = user.UserName;
                userDto.Name = user.ClientProfile.Name;
                userDto.Id = user.Id;
                userDto.Email = user.Email;
                foreach (var f in user.ClientProfile.Friends)
                {
                    userDto.Friends.Add(new FriendsDTO { Id = f.Id, FriendId = f.FriendId });
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
                userDto.Info = user.ClientProfile.Info;
                userDto.Address = user.ClientProfile.Address;
                userDto.Age = user.ClientProfile.Age;
                userDto.UserName = user.UserName;
                userDto.Name = user.ClientProfile.Name;
                userDto.Email = user.Email;
                userDto.Id = user.Id;
                foreach (var f in user.ClientProfile.Friends)
                {
                    userDto.Friends.Add(new FriendsDTO{Id = f.Id, FriendId = f.FriendId});
                }
                return userDto;
            }
            return null;
        }

        public async Task<bool> EditProfile(string id,string name, string email, string info, string address,int age)
        {
            try
            {
                Database.UserManager.FindByIdAsync(id).Result.ClientProfile.Address = address;
                Database.UserManager.FindByIdAsync(id).Result.Email = email;
                Database.UserManager.FindByIdAsync(id).Result.ClientProfile.Age = age;
                Database.UserManager.FindByIdAsync(id).Result.UserName = email;
                Database.UserManager.FindByIdAsync(id).Result.ClientProfile.Age = age;
                Database.UserManager.FindByIdAsync(id).Result.ClientProfile.Name = name;
                Database.UserManager.FindByIdAsync(id).Result.ClientProfile.Info = info;
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
                    ClientProfile = await Database.ClientManager.Find(id)
                };
                Database.FriendsManager.Create(f1);
                Database.UserManager.FindByIdAsync(friendId).Result.ClientProfile.Friends.Add(f1);
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
            Database.Dispose();
        }
    }
}

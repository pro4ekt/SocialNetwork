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
                MemberProfile clientProfile = new MemberProfile { Id = user.Id, Address = userDto.Address, Name = userDto.Name, Age = userDto.Age, Info = userDto.Info};
                Database.MemberManager.Create(clientProfile);
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
                userDto.Info = user.ClientProfile.Info;
                userDto.Age = user.ClientProfile.Age;
                userDto.UserName = user.UserName;
                userDto.Name = user.ClientProfile.Name;
                userDto.Id = user.Id;
                userDto.Email = user.Email;
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
                userDto.Info = user.ClientProfile.Info;
                userDto.Age = user.ClientProfile.Age;
                userDto.UserName = user.UserName;
                userDto.Name = user.ClientProfile.Name;
                userDto.Email = user.Email;
                userDto.Id = user.Id;
                return userDto;
            }
            return null;
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

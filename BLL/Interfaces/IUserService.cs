using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Infrastructure;
using DAL.Entities;

namespace BLL.Interfaces
{
    public interface IUserService : IDisposable
    {
        Task<OperationDetails> Create(UserDTO userDto);
        Task<bool> RemoveUser(string id);
        Task<bool> BanUser(string id);
        Task<bool> UnBanUser(string id);
        Task<ClaimsIdentity> Authenticate(UserDTO userDto);
        Task<UserDTO> FindByEmail(string email);
        Task<UserDTO> FindById(string id);
        Task<UserDTO> FindByName(string userName);
        Task<bool> EditProfile(string id, string name, string email, string info, string address, int age, UserDTO user);
        Task<bool> AddFriend(string id, string friendId);
        Task<bool> RemoveFriend(string id, string friendId);
        Task<bool> CheckPassword(string email, string password);
        Task SaveMessage(MessagesDTO messageDto);
        Task RemoveMessage(string id);
        Task SetInitialData(List<UserDTO> usersDto, List<string> roles);
    }
}

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
        Task<ClaimsIdentity> Authenticate(UserDTO userDto);
        Task<UserDTO> Find(UserDTO userDto);
        Task SetInitialData(UserDTO adminDto, List<string> roles);
        Task SetInitialData(List<UserDTO> usersDto, List<string> roles);
    }
}

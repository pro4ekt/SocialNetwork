using System;
using System.Threading.Tasks;
using DAL.Identity;

namespace DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; }
        IClientManager ClientManager { get; }
        IFriendsManager FriendsManager { get; }
        ApplicationRoleManager RoleManager { get; }
        Task SaveAsync();
    }
}

using BLL.Interfaces;
using DAL.Repositories;

namespace BLL.Services
{
    public class ServiceCreator : IServiceCreator
    {
        /// <summary>
        /// класс реализующий абстрактную фабрику
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public IUserService CreateUserService(string connection)
        {
            return new UserService(new IdentityUnitOfWork(connection));
        }
    }
}

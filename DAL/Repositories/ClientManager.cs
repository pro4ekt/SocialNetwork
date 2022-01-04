using System.Threading.Tasks;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    /// <summary>
    /// Менеджер клиентов(по сути выполняет роль репозитория для клиентов)
    /// </summary>
    public class ClientManager : IClientManager
    {
        public ApplicationContext Database { get; set; }
        public ClientManager(ApplicationContext db)
        {
            Database = db;
        }

        public void Create(ClientProfile item)
        {
            Database.ClientProfiles.Add(item);
            Database.SaveChanges();
        }

        public async Task<ClientProfile> Find(string id)
        {
            return await Database.ClientProfiles.FindAsync(id);
        }

        public void Remove(ClientProfile item)
        {
            Database.ClientProfiles.Remove(item);
            Database.SaveChanges();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}

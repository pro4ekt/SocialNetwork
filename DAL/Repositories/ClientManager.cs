using System.Threading.Tasks;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
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

        public Task<ClientProfile> Find(string id)
        {
            return Database.ClientProfiles.FindAsync(id);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}

using System.Collections.Generic;
using System.Linq;
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

        public ClientProfile Find(string id)
        {
            return Database.ClientProfiles.Find(id);
        }

        public void Remove(ClientProfile item)
        {
            Database.ClientProfiles.Remove(item);
            Database.SaveChanges();
        }

        public List<ClientProfile> GetAll()
        {
            return Database.ClientProfiles.ToList();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}

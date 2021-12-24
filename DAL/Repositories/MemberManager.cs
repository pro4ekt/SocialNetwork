using System.Threading.Tasks;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class MemberManager : IMemberManager
    {
        public ApplicationContext Database { get; set; }
        public MemberManager(ApplicationContext db)
        {
            Database = db;
        }

        public void Create(MemberProfile item)
        {
            Database.ClientProfiles.Add(item);
            Database.SaveChanges();
        }

        public Task<MemberProfile> Find(string id)
        {
            return Database.ClientProfiles.FindAsync(id);
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}

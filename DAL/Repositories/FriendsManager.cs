using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class FriendsManager : IFriendsManager
    {
        public ApplicationContext Database { get; set; }
        public FriendsManager(ApplicationContext db)
        {
            Database = db;
        }

        public void Create(Friends item)
        {
            Database.Friends.Add(item);
            Database.SaveChanges();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    /// <summary>
    /// Менеджер дружбы(по сути выполняет роль репозитория для дружб)
    /// </summary>
    public class FriendsManager : IFriendsManager
    {
        public ApplicationContext Database { get; set; }
        public FriendsManager(ApplicationContext db)
        {
            Database = db;
        }

        public void Create(Friends item)
        {
            Database.Friends.Add(new Friends
            {
                Id = item.Id,
                FriendId = item.FriendId
            });
            Database.Friends.Add(new Friends
            {
                Id = item.FriendId,
                FriendId = item.Id,
            });
            Database.SaveChanges();
        }

        public void Remove(Friends item)
        {
            var a = Database.Friends.SingleOrDefault(i => i.Id == item.Id && i.FriendId == item.FriendId);
            Database.Friends.Remove(a);
            var b = Database.Friends.SingleOrDefault(i => i.Id == item.FriendId && i.FriendId == item.Id);
            Database.Friends.Remove(b);
            //Database.Friends.RemoveRange(Database.Friends.Where(f => (f.Id == item.Id && f.FriendId == item.FriendId)));
            //Database.Friends.RemoveRange(Database.Friends.Where(f => (f.Id == item.FriendId && f.FriendId == item.Id)));
            Database.SaveChanges();
        }

        public List<Friends> GetAll()
        {
            return Database.Friends.ToList();
        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}

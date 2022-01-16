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

        public Friends Find(string id, string friendId)
        {
            var f = GetAll();
            var f1 = f.Where(c => c.Id == id && c.FriendId == friendId).ToList()[0];
            return f1;
        }
        public void Remove(Friends item)
        {
            Database.Friends.Remove(item);
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

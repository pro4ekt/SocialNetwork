using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.EF;
using DAL.Entities;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class MessageManager : IMessageManager
    {
        public ApplicationContext Database { get; set; }
        public MessageManager(ApplicationContext db)
        {
            Database = db;
        }

        public void Create(Messages item)
        {
            Database.Messages.Add(item);
            Database.SaveChanges();
        }
        public void Remove(Messages item)
        {
            var m = Database.Messages.SingleOrDefault(a => a.MessageId == item.MessageId);
            Database.Messages.Remove(m);
            //Database.Messages.Attach(item);
            //Database.Entry(item).State = EntityState.Deleted;
            Database.SaveChanges();
        }
        public Messages Find(string id)
        {
            return Database.Messages.SingleOrDefault(m=>m.MessageId == id);
        }
        public List<Messages> GetAll()
        {
            return Database.Messages.ToList();
        }
        public void Dispose()
        {
            Database.Dispose();
        }

    }
}

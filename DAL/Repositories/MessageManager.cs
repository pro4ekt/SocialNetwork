using System;
using System.Collections.Generic;
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
            Database.Messages.Remove(item);
            Database.SaveChanges();
        }
        public async Task<Messages> Find(string id)
        {
            return await Database.Messages.FindAsync(id);
        }
        public void Dispose()
        {
            Database.Dispose();
        }
    }
}

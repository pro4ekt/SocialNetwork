using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IApplicationContext : IDisposable
    {
        DbSet<ClientProfile> ClientProfiles { get; set; }
        DbSet<Friends> Friends { get; set; }
        DbSet<Messages> Messages { get; set; }
        int SaveChanges();
    }
}

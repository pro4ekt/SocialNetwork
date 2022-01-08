using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using DAL.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using static System.Data.Entity.DbContext;

namespace DAL.EF
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>, IDbContextFactory<ApplicationContext>
    {
        public ApplicationContext()
        {

        }
        public ApplicationContext(string conectionString) : base(conectionString) {}

        public DbSet<ClientProfile> ClientProfiles { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<Messages> Messages { get; set; }

        public ApplicationContext Create()
        {
            throw new System.NotImplementedException();
        }
    }
}

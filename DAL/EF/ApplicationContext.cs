using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using DAL.Entities;
using DAL.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DAL.EF
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser>, IApplicationContext
    {
        public ApplicationContext() : base("AttachDbFilename=TestContext.mdf")
        {

        }
        public ApplicationContext(string conectionString) : base(conectionString) {}

        public virtual DbSet<ClientProfile> ClientProfiles { get; set; }
        public virtual DbSet<Friends> Friends { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
    }
}

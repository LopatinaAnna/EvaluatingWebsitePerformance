using EvaluatingWebsitePerformance.Data.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace EvaluatingWebsitePerformance.Data
{
    public class DbContext : IdentityDbContext<ApplicationUser>
    {
        public DbContext() : base("DefaultConnection")
        {
        }

        public static DbContext Create()
        {
            return new DbContext();
        }
        public virtual DbSet<BaseRequest> SharedFiles { get; set; }
        public virtual DbSet<SitemapRequest> Storages { get; set; }
        public virtual DbSet<User> ClientProfiles { get; set; }
    }

}
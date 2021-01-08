using EvaluatingWebsitePerformance.Data.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace EvaluatingWebsitePerformance.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<BaseRequest> BaseRequests { get; set; }
        public virtual DbSet<SitemapRequest> SitemapRequests { get; set; }
    }
}
namespace EvaluatingWebsitePerformance.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<EvaluatingWebsitePerformance.Data.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(EvaluatingWebsitePerformance.Data.ApplicationDbContext context)
        {
        }
    }
}

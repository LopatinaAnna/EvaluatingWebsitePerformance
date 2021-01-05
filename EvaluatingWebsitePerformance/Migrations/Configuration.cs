namespace EvaluatingWebsitePerformance.Migrations
{
    using System.Data.Entity.Migrations;
    using EvaluatingWebsitePerformance.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DbContext context)
        {
        }
    }
}

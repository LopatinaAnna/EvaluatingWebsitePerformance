using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EvaluatingWebsitePerformance.Startup))]

namespace EvaluatingWebsitePerformance
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
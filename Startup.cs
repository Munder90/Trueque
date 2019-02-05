using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TAT001.Startup))]
namespace TAT001
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

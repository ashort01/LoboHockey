using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UNMHockeySite.Startup))]
namespace UNMHockeySite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

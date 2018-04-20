using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TrueMarbleWeb.Startup))]
namespace TrueMarbleWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}

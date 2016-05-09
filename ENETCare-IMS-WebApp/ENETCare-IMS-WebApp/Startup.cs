using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ENETCare_IMS_WebApp.Startup))]
namespace ENETCare_IMS_WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

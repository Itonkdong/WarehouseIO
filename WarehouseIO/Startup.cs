using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WarehouseIO.Startup))]
namespace WarehouseIO
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

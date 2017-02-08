using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PixelApp.Startup))]
namespace PixelApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

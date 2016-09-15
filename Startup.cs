using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ControlWorkMVC1.Startup))]
namespace ControlWorkMVC1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

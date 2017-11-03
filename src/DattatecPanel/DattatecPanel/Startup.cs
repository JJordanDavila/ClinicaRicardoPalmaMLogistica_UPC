using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DattatecPanel.Startup))]
namespace DattatecPanel
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

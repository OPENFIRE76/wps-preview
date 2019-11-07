using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(wpsPreview.Startup))]
namespace wpsPreview
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

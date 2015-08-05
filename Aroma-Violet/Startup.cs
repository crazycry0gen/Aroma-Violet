using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Aroma_Violet.Startup))]
namespace Aroma_Violet
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

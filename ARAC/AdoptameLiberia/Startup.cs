using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdoptameLiberia.Startup))]
namespace AdoptameLiberia
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

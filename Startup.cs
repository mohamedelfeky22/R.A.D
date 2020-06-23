using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(R.A.D.Startup))]
namespace R.A.D
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}

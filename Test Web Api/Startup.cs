using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Test_Web_Api.Startup))]
namespace Test_Web_Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

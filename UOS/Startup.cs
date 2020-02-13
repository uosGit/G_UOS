using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UOS.Models;
using Microsoft.CSharp;
using Microsoft.Owin.Security.OAuth;
using UOS.Provider;
using System.Web.Http;
using Microsoft.Owin.Cors;
using System.Web;



[assembly: OwinStartup(typeof(UOS.Startup))]


namespace UOS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}

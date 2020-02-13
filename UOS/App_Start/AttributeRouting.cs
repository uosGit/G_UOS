using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof(UOS.AttributeRouting), "Start")]

namespace UOS 
{
    public static class AttributeRouting 
	{
		public static void RegisterRoutes(RouteCollection routes) 
		{    
			// See http://github.com/mccalltd/AttributeRouting/wiki for more options.
			// To debug routes locally using the built in ASP.NET development server, go to /routes.axd
            
			routes.MapAttributeRoutes();
		}

        public static void Start() 
		{
            RegisterRoutes(RouteTable.Routes);
        }
    }
}
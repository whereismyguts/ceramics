using System.Web.Mvc;
using System.Web.Routing;

namespace AppHarborMongoDBDemo {
    public class MvcApplication: System.Web.HttpApplication {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
             name: "Item",
             url: "Item/{id}",
             defaults: new { controller = "Home", action = "Item" }
         );
            routes.MapRoute(
             name: "Edit",
             url: "Edit/{id}",
             defaults: new { controller = "Manage", action = "Edit" }
         );
            routes.MapRoute(
          name: "Work",
          url: "Work",
          defaults: new { controller = "Home", action = "Work" }
      );

            routes.MapRoute(
        name: "Contact",
        url: "Contact",
        defaults: new { controller = "Home", action = "Contact" }
    );

            routes.MapRoute(
    name: "Remove",
    url: "Remove/{id}",
    defaults: new { controller = "Manage", action = "Remove" }
);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}

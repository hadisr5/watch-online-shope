using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineShop
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            routes.MapRoute(
name: "Product",
url: "Home/product/{id}/{title}",
defaults: new { controller = "Home", action = "Product", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "page",
url: "Home/Page/{id}/{title}",
defaults: new { controller = "Home", action = "Page", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "category",
url: "Home/category/{id}/{title}",
defaults: new { controller = "Home", action = "Category", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "MainCat",
url: "Home/MainCat/{id}/{title}",
defaults: new { controller = "Home", action = "MainCat", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "search",
url: "Home/search/{title}",
defaults: new { controller = "Home", action = "Search", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "post",
url: "blog/details/{id}/{title}",
defaults: new { controller = "blog", action = "details", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "Brands",
url: "Home/Brands/{id}/{title}",
defaults: new { controller = "Home", action = "Brands", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "brandList",
url: "Home/brandList/{id}/{title}",
defaults: new { controller = "Home", action = "brandList", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "sellers",
url: "Home/sellers/{id}/{title}",
defaults: new { controller = "Home", action = "sellers", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "blogCategory",
url: "Blog/category/{id}/{title}",
defaults: new { controller = "Blog", action = "Category", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "Promotions",
url: "Promotions/index/{id}/{title}",
defaults: new { controller = "Promotions", action = "Index", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "filter",
url: "home/filter/{id}/{title}",
defaults: new { controller = "home", action = "filter", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);
            routes.MapRoute(
name: "contact",
url: "home/contact/{title}",
defaults: new { controller = "home", action = "contact", title = UrlParameter.Optional },
constraints: new { id = @"\d+" }
);


        }
    }
}

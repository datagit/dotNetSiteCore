using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace CoffeeShop.Pipelines
{
    public class RegisterCustomRoute
    {
        public virtual void Process(PipelineArgs args)
        {
            RouteTable.Routes.MapRoute("CustomRoute", "detail/{id}", new
            {
                controller = "Home",
                action = "Detail",
                id = UrlParameter.Optional 
            });
        }
    }
}
using CoffeeShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShop.Controllers
{
    public class DefaultController : Controller
    {
        // GET: Default
        public ActionResult Index()
        {
            HomeViewModel result = null;
            var item = Sitecore.Context.Item;
            if (item != null) {
                result = new HomeViewModel {
                    Title = item.Fields["Title"].Value,
                    Description = item.Fields["Description"] == null ? string.Empty : item.Fields["Description"].Value

                };

            }
            return View(result);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Traffix.Common.Providers;
using Traffix.Common.Providers.Logging;

namespace Traffix.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {}

        public ActionResult Index()
        {
            return View();
        }
    }
}
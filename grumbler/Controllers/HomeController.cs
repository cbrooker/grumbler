using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace grumbler.Controllers {
    public class HomeController : Controller {
        Projects _projects;
        public HomeController() {
            _projects = new Projects();
        }
        public ActionResult Index() {
            return View(_projects.All());
        }

    }
}

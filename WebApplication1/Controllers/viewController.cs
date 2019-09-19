using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    public class viewController : Controller
    {
        // GET: view
        [Route(Name = "v1")]
        public ActionResult Index()
        {
            return Content("123");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RaysBlog.Web.Areas.Manage.Controllers
{
    public class LoginController : Controller
    {
        [Area("Manage")]
        [Route("Ray/Login")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
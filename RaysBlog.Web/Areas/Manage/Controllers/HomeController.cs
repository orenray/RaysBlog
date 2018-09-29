using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;

namespace RaysBlog.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("ray/index")]
    public class HomeController : Controller
    {
        private readonly CategoryRepository _categoryService;
        public HomeController()
        {
            _categoryService = new CategoryRepository();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("item/{id?}")]
        public IActionResult Index(string id,int pageIndex,int pageSize,bool ascending=true)
        {
            if (!string.IsNullOrEmpty(id))
                return PartialView("_CategoryList",(id,pageIndex,pageSize));
            return View("/ray/index");
        }
    }
}
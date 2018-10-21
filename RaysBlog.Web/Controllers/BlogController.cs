using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaysBlog.Model;
using RaysBlog.Repository;

namespace RaysBlog.Web.Controllers
{
    [Route("strawman/blog")]
    public class BlogController : Controller
    {
        private readonly CategoryRepository  categoryRepository;
        private readonly ArticleRepository  articleRepository;
        private readonly IHttpContextAccessor accessor;
        private readonly int total = 0;
        public BlogController(IHttpContextAccessor accessor)
        {
            categoryRepository = new CategoryRepository();
            articleRepository = new ArticleRepository();
            total= articleRepository.GetTotalCount();
            this.accessor = accessor;
        }
        public IActionResult Index()
        {
            ViewBag.Pz = 1;
            ViewBag.TotalRecord = total;
            var cates= categoryRepository.GetCategorys();
            var arts = articleRepository.GetPagerByViewNum(1, 5);
            (IEnumerable<BlogCategory> cas, IEnumerable<BlogArticle> ats) blogData= (cates, arts);
            return View( blogData);
        }
        [HttpPost]
        public IActionResult Index(int id)
        {
            ViewBag.Pz = id;
            return ViewComponent("ArticleSection",new { m=id});
        }
        [Route("article/{id}")]
        public IActionResult Article(int id)
        {
            accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var art = articleRepository.Get(id);
            return View("Article",art);
        }
    }
}
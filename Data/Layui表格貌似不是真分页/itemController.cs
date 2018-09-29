using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;
using RaysBlog.Repository.Interface;

namespace RaysBlog.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("Ray/item")]
    public class ItemController : Controller
    {
        private readonly ICategoryRepository _categoryService;
        public ItemController()
        {
            _categoryService = new CategoryRepository();
        }

        public IActionResult Index(string id)
        {
            ViewBag.TypeId = id;
            ViewBag.PageIndex = 1;
            return View();
        }
        //[HttpPost]//需要改下 不用viewComponent
        //public IActionResult Index(string id,int pageIndex, bool ascending = true)
        //{
        //    ViewBag.TypeId = id;
        //    ViewBag.PageIndex = pageIndex;
        //    return ViewComponent("Category", new { id, pageIndex, ascending });
        //}
        [HttpPost]
        public IActionResult Index(string id,string aaa)
        {
            ViewBag.TypeId = id;
            return Json(GetPeropers(id));
        }
        [HttpPost]
        [Route("List")]
        public IActionResult List(string id, int pageIndex, bool ascending = true)
        {
            var pageSize = 6;
            var totalCount = _categoryService.GetCount();
            var categoryInfo = _categoryService.GetEntities(pageIndex, pageSize, ascending);
            var list = new List<dynamic>();

            foreach (var item in categoryInfo)
            {
                dynamic m = new {
                    categoryName = item.CategoryName,
                    id = item.Id,
                    no = item.No,
                    createDate = item.CreateDate.ToString("yyyy-MM-dd"),
                    isEnabled =item.IsEnabled?"是":"否",
                    remark=item.Remark
                };
                list.Add(m);
            }
            var info =  new { code=0,msg="ok",count=totalCount,data=list};
            //(IEnumerable<RaysBlog.Model.BlogCategory>, int, int, string) tupleData = (categoryInfo, totalCount, pageSize, id);
            return Json(info);
        }
        private List<dynamic> GetPeropers(string id)
        {
            List<dynamic> fieldInfos = new List<dynamic>();
            switch (id)
            {
                case "1":
                    RaysBlog.Model.BlogCategory category = new Model.BlogCategory();
                    fieldInfos.Add(new { field = nameof(category.Id).Substring(0,1).ToLower()+ nameof(category.Id).Substring(1), title = "ID", width = 50, sort = true,hide=true, @fixed = "left", unresize = true });
                    fieldInfos.Add(new { field = nameof(category.No).Substring(0, 1).ToLower() + nameof(category.No).Substring(1), title = "序号", width = 80, sort = true, unresize = true });
                    fieldInfos.Add(new { field = nameof(category.CategoryName).Substring(0, 1).ToLower() + nameof(category.CategoryName).Substring(1), title = "分类名称", width = 200});
                    fieldInfos.Add(new { field = nameof(category.CreateDate).Substring(0, 1).ToLower() + nameof(category.CreateDate).Substring(1), title = "创建时间", width = 200, sort = true, unresize = true });
                    fieldInfos.Add(new { field = nameof(category.IsEnabled).Substring(0, 1).ToLower() + nameof(category.IsEnabled).Substring(1), title = "是否启用", width = 80, unresize = true });
                    fieldInfos.Add(new { field = nameof(category.Remark).Substring(0, 1).ToLower() + nameof(category.Remark).Substring(1), title = "备注", width = 200});
                    break;
            }
            fieldInfos.Add(new { field = "right", width=178, align="center", toolbar= "#toolBar" });
            return fieldInfos;
        }
    }
}
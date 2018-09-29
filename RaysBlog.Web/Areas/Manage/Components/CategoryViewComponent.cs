using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaysBlog.Web.Areas.Manage.Components
{
    public class CategoryViewComponent:ViewComponent
    {
        private readonly CategoryRepository _categoryService;
        public CategoryViewComponent()
        {
            _categoryService = new CategoryRepository();
        }
        public  IViewComponentResult Invoke(string id,int pageIndex,bool ascending=true)
        {
            var pageSize = 5;
            var totalCount= _categoryService.GetTotalCount();
            var categoryInfo = _categoryService.GetPager(pageIndex, pageSize);
            var tupleData= (categoryInfo,totalCount,pageSize,id);
               return  View(tupleData);
        }
    }
}

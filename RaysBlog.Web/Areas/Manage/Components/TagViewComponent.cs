using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaysBlog.Web.Areas.Manage.Components
{
    public class TagViewComponent:ViewComponent
    {
        private readonly TagRepository _tagService;
        public TagViewComponent()
        {
            _tagService = new TagRepository();
        }
        public  IViewComponentResult Invoke(string id,int pageIndex,bool ascending=true)
        {
            var pageSize = 5;
            var totalCount= _tagService.GetTotalCount();
            var tagInfo = _tagService.GetPager(pageIndex, pageSize);
            var tupleData= (tagInfo, totalCount,pageSize,id);
               return  View(tupleData);
        }
    }
}

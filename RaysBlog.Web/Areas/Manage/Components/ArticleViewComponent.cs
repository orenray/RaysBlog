using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaysBlog.Web.Areas.Manage.Components
{
    public class ArticleViewComponent : ViewComponent
    {
        private readonly ArticleRepository _articleService;
        //private readonly IMapper mapper;
        //public ArticleViewComponent(IMapper mapper)
        //{
        //    //this.mapper = mapper;
        //    _articleService = new ArticleRepository();
        //}
        public ArticleViewComponent()
        {
            _articleService = new ArticleRepository();
        }
        public IViewComponentResult Invoke(string id, int pageIndex, bool ascending = true)
        {
            var pageSize = 5;
            var totalCount = _articleService.GetTotalCount();
            var articleInfo = _articleService.GetPagerByKeywords(pageIndex, pageSize);
            //var infos = mapper.Map<IEnumerable<ArticleDTO>>(articleInfo);
            var tupleData = (articleInfo, totalCount, pageSize, id);
            return View(tupleData);
        }
    }
}

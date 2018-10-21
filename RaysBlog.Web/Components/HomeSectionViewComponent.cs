using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaysBlog.Web.Components
{
    public class HomeSectionViewComponent:ViewComponent
    {
        private readonly RaysBlog.Repository.ArticleRepository articleRepository;
        public HomeSectionViewComponent()
        {
            articleRepository = new Repository.ArticleRepository();
        }
        public IViewComponentResult Invoke()
        {
            IEnumerable<(int id,string imgPath,string title,string date,string desc)> articles = articleRepository.GetPagerByKeywords(1, 3, "").Select<RaysBlog.Model.BlogArticle,ValueTuple<int,string,string,string,string>>(s=>
            {
                return (s.ArticleId,s.TitleImgPath,s.ArticleName,s.PostDate.ToString("yyyy-MM-dd"),s.Body.Substring(0,s.Body.Length>20?20:s.Body.Length)+"...");
            });
            return View(articles);
        }
    }
}

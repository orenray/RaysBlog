using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaysBlog.Web.Components
{
    public class ArticleSectionVIewComponent:ViewComponent
    {
        private readonly ArticleRepository articleRepository;
        public ArticleSectionVIewComponent()
        {
            articleRepository = new ArticleRepository();
        }
        public IViewComponentResult Invoke(int m)
        {
            int pageSize = 4;
            IEnumerable<(int id,string imgPath,(string title,string desc,string category,string tag,DateTime postDate,int viewNum,int commentNum) articleDesc)> arts= articleRepository.GetPagerByKeywords(m, pageSize, "").Select<RaysBlog.Model.BlogArticle,(int,string,(string,string,string,string,DateTime,int,int))>(s=> 
            {
                return (s.ArticleId,s.TitleImgPath, (s.ArticleName, s.Body, s.Category.CategoryName, s.Tag?.TagName??"", s.PostDate, s.ViewNum, s.CommentNum));
            });
            return View(arts);
        }
    }
}

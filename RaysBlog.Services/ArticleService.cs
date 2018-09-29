using RaysBlog.Model;
using RaysBlog.Repository;
using RaysBlog.Services.Base;
using RaysBlog.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Services
{
    #region MyRegion1
    //public class ArticleSerivce : BaseService<BlogArticle>, IArticleService
    //{
    //    private readonly IArticleRepository dal;
    //    public ArticleSerivce(IArticleRepository dal)
    //    {
    //        this.dal = dal;
    //        base.baseDal = dal;
    //    }

    //    public IEnumerable<BlogArticle> GetEntitiesByKeyword(string keyword, int pageIndex, int pageSize, bool ascending = true)
    //    {
    //        return dal.GetEntitiesByKeyword(keyword, pageIndex, pageSize, ascending);
    //    }

    //    public IEnumerable<BlogArticle> GetEntitiesByTag(string tag, int pageIndex, int pageSize, bool ascending = true)
    //    {
    //        return dal.GetEntitiesByTag(tag, pageIndex, pageSize, ascending);
    //    }
    //} 
    #endregion
    #region MyRegion2
    public class ArticleSerivce 
    {
        private readonly ArticleRepository  articleRepository;
        public ArticleSerivce()
        {
            this.articleRepository = new ArticleRepository();
        }

        public BlogArticle Get(int id)
        {
            return articleRepository.Get(id);
        }
        
      
    }
    #endregion
}

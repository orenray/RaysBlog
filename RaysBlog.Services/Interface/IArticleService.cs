using RaysBlog.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Services.Interface
{
    public interface IArticleService : IBaseService<BlogArticle>
    {
        IEnumerable<BlogArticle> GetEntitiesByTag(string tag, int pageIndex, int pageSize, bool ascending = true);
        IEnumerable<BlogArticle> GetEntitiesByKeyword(string keyword, int pageIndex, int pageSize, bool ascending = true);
    }
}

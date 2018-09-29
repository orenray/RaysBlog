using RaysBlog.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RaysBlog.Repository.Interface
{
    public interface IArticleRepository:IBaseRepository<BlogArticle> 

    {
        IEnumerable<BlogArticle> GetEntitiesByTag(string tag,int pageIndex, int pageSize, bool ascending = true);
        IEnumerable<BlogArticle> GetEntitiesByKeyword(string keyword, int pageIndex, int pageSize, bool ascending = true);
    }
}

using RaysBlog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using RaysBlog.Model;
using System.Data.Common;
//using DapperExtensions;
using System.Linq;
using Dapper;
using System.Threading.Tasks;

namespace RaysBlog.Repository
{
    public class ArticleRepository : BaseRepository<BlogArticle>, IArticleRepository
    {
        #region using DapperExtensions
        //public IEnumerable<BlogArticle> GetEntitiesByKeyword(string keyword, int pageIndex, int pageSize, bool ascending = true)
        //{
        //    var key = keyword.ToLowerInvariant();
        //    using (var conn = ConnectionFactory.GetOpenConnection())
        //    {
        //        var pgMain = new PredicateGroup { Operator = GroupOperator.And, Predicates = new List<IPredicate>() };

        //        var pga = Predicates.Field<BlogArticle>(b => b.IsPublished, Operator.Eq, true);

        //        pgMain.Predicates.Add(pga);

        //        var pgb = new PredicateGroup { Operator = GroupOperator.Or, Predicates = new List<IPredicate>() };
        //        pgb.Predicates.Add(Predicates.Field<BlogArticle>(b => b.Body.ToLowerInvariant().Contains(key), Operator.Eq, true));
        //        pgb.Predicates.Add(Predicates.Property<BlogArticle, BlogCategory>(a => a.Id, Operator.Eq, b => b.Id));
        //        pgb.Predicates.Add(Predicates.Field<BlogArticle>(b => b.ArticleName.ToLowerInvariant().Contains(key), Operator.Eq, true));

        //        pgMain.Predicates.Add(pgb);

        //        var blogInfo = conn.GetPage<BlogArticle>(pgMain, new List<ISort> { Predicates.Sort<BlogArticle>(b => b.Id, ascending) }, pageIndex - 1, pageSize);
        //        return blogInfo;
        //    }
        //}

        //public IEnumerable<BlogArticle> GetEntitiesByTag(string tag, int pageIndex, int pageSize, bool ascending = true)
        //{
        //    #region MyRegion
        //    //var lowerTag = tag.ToLowerInvariant();
        //    //using (var conn = ConnectionFactory.GetOpenConnection())
        //    //{
        //    //    List<IPredicate> predicates = new List<IPredicate>()
        //    //    {
        //    //        Predicates.Field<BlogArticle>(b=>b.IsPublished,Operator.Eq,true),
        //    //        Predicates.Property<BlogArticle,BlogCategory>(a=>a.CategoryId,Operator.Eq,b=>b.Id)
        //    //    };

        //    //    var pages = conn.GetPage<BlogArticle>(Predicates.Group(GroupOperator.And, predicates.ToArray()), new List<ISort>
        //    //    {
        //    //        Predicates.Sort<BlogArticle>(b=>b.Id,ascending)
        //    //    }, pageIndex, pageSize);               
        //    //    return pages;
        //    //} 
        //    #endregion
        //    using (var conn = ConnectionFactory.GetOpenConnection())
        //    {

        //        return null;
        //    }
        //}
        //public override IEnumerable<BlogArticle> GetEntities(int pageIndex, int pageSize, bool ascending = true)
        //{
        //    if (pageIndex < 1) pageIndex = 1;
        //    if (pageSize > 10) pageSize = 10;
        //    using (var conn = ConnectionFactory.GetOpenConnection())
        //    {
        //        var count = conn.Count<BlogArticle>(null);

        //        var pages = conn.GetPage<BlogArticle>(Predicates.Field<BlogArticle>(b => b.IsPublished, Operator.Eq, true), new List<ISort> { Predicates.Sort<BlogArticle>(s => s.Id, ascending) }, pageIndex - 1, pageSize).ToList();
        //        return pages;
        //    }
        //} 
        #endregion
        public BlogArticle Get<BlogArticle>(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return conn.QueryFirstOrDefault<BlogArticle>("SELECT * FROM BlogArticle WHERE Id=@id", new { id });
            }
        }
        public async Task<BlogArticle> GetAsync<BlogArticle>(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<BlogArticle>("SELECT * FROM BlogArticle WHERE Id=@id", new { id });
            }
        }

        public IEnumerable<BlogArticle> GetEntitiesByKeywords(int pageIndex, int pageSize, string condition)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT TOP (@PageSize) Paged.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY Id) AS [No],* FROM BlogArticle WHERE body LIKE '%@CondStr%') AS Paged WHERE [No] > (@Page - 1) * @PageSize";
                return conn.Query<BlogArticle>(sql, new { PageSize = pageSize, Page = pageIndex, CondStr = condition });
            }
        }
        public async Task<IEnumerable<BlogArticle>> GetEntitiesByKeywordsAsync(int pageIndex, int pageSize, string condition)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT TOP (@PageSize) Paged.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY Id) AS [No],* FROM BlogArticle WHERE body LIKE '%@CondStr%') AS Paged WHERE [No] > (@Page - 1) * @PageSize";
                return await conn.QueryAsync<BlogArticle>(sql, new { PageSize = pageSize, Page = pageIndex, CondStr = condition });
            }
        }
    }
}

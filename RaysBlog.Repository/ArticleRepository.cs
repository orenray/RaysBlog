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
    public class ArticleRepository //:IArticleRepository
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
        private int GetExistCount(string sql,object parameter)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                var result = conn.ExecuteScalar<int>(sql, parameter);
               return result;
            }
        }
        public int GetTotalCount()
        {
            return GetExistCount("select count(*) from blogarticle where ispublished=1",null);
        }
        public int GetMaxId()
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                return conn.ExecuteScalar<int>("select max(articleId) from blogarticle");
            }
        }
        public BlogArticle Get(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                string sql = @"SELECT ba.ArticleId,ba.Body,ba.PostDate,ba.Remark,ba.ArticleName,ba.IsPublished,ba.CaId,ba.TitleImgPath,ba.ViewNum,CommentNum=(select count(*) from blogcomment where ArticleId=@id),bt.*,bc.*
                    FROM blogarticle AS ba LEFT JOIN blogtag AS bt ON bt.ArId=ba.ArticleId 
                    INNER JOIN blogcategory AS bc ON ba.CaId=bc.CategoryId
                    WHERE ba.ArticleId=@id AND ba.IsPublished=1";
                //var lookup = new Dictionary<int, BlogArticle>();
               var result=  conn.Query<BlogArticle, BlogTag, BlogCategory, BlogArticle>(sql, (ba, bt, bc) =>
                {
                    ba.Category = bc;
                    ba.Tag = bt;
                    return ba;
                }, new { id }, splitOn: "ArticleId,TagId,CategoryId").AsQueryable();//延迟加载
                //return lookup.Values.Select(s=>s).FirstOrDefault();
                return result.FirstOrDefault();
            }
        }
        public async Task<BlogArticle> GetAsync(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                //string sql = @"SELECT ba.ArticleId,ba.Body,ba.PostDate,ba.Remark,ba.ArticleName,ba.IsPublished,ba.CaId,ba.TitleImgPath,ba.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=ba.ArticleId ),bt.*,bc.*
                //    FROM BlogArticle AS ba INNER JOIN BlogTag AS bt ON bt.ArId=ba.ArticleId 
                //    INNER JOIN BlogCategory AS bc ON ba.CaId=bc.CategoryId
                //    WHERE ba.ArticleId=@id AND ba.IsPublished=1";
                string sql = @"SELECT ba.ArticleId,ba.Body,ba.PostDate,ba.Remark,ba.ArticleName,ba.IsPublished,ba.CaId,ba.TitleImgPath,ba.ViewNum,CommentNum=(select count(*) from blogcomment where ArticleId=@id),bt.*,bc.*
                    FROM blogarticle AS ba LEFT JOIN blogtag AS bt ON bt.ArId=ba.ArticleId 
                    INNER JOIN blogcategory AS bc ON ba.CaId=bc.CategoryId
                    WHERE ba.ArticleId=@id AND ba.IsPublished=1";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result=  await conn.QueryAsync<BlogArticle, BlogTag, BlogCategory, BlogArticle>(sql, (ba, bt, bc) =>
                {
                    ba.Category = bc;
                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))//内联变量
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    ba.Tag = bt;
                    return ba;
                }, new { id }, splitOn: "ArticleId,TagId,CategoryId");//延迟加载
                //return lookup.Values.FirstOrDefault();
                return result.AsQueryable().FirstOrDefault();
            }
        }
        public IEnumerable<BlogArticle> GetPagerByViewNum(int pageIndex, int pageSize)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                //string sql = @"SELECT t1.*,bt.*,bc.*
                //                FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId,paged.TitleImgPath,paged.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=paged.ArticleId )
                //                FROM (SELECT TOP  100 PERCENT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                //                FROM BlogArticle WHERE IsPublished=1  order by ViewNum desc) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                //                LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                //                LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                var mysql = @"SELECT T1.*,bt.*,bc.* 
FROM (SELECT ArticleId, Body, PostDate, Remark, ArticleName, IsPublished,CaId, TitleImgPath, ViewNum, CommentNum=(SELECT COUNT(*) FROM blogcomment  WHERE ArticleId=ArticleId )
FROM blogarticle  WHERE IsPublished = 1  ORDER BY ViewNum DESC LIMIT @pageIndex, @pageSize) AS T1
LEFT JOIN blogtag AS bt ON bt.arId = T1.articleID
INNER JOIN blogcategory AS bc ON bc.categoryId = T1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result = conn.Query<BlogArticle, BlogTag, BlogCategory, BlogArticle>(mysql, (ba, bt, bc) =>
                {
                    ba.Category = bc;

                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    //return bat;
                    ba.Tag = bt;
                    return ba;
                }, new { PageSize = pageSize, PageIndex = pageIndex}, splitOn: "ArticleId,TagId,CategoryId").AsQueryable();
                //return lookup.Values;
                return result;
            }
        }
        public async Task<IEnumerable<BlogArticle>> GetPagerByViewNumAsync(int pageIndex, int pageSize)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {

                //string sql = @"SELECT t1.*,bt.*,bc.*
                //            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId,paged.TitleImgPath,paged.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=paged.ArticleId )
                //            FROM (SELECT TOP  100 PERCENT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                //            FROM BlogArticle WHERE IsPublished=1  order by ViewNum desc) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                //            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                //            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                var mysql = @"SELECT T1.*,bt.*,bc.* 
FROM (SELECT ArticleId, Body, PostDate, Remark, ArticleName, IsPublished,CaId, TitleImgPath, ViewNum, CommentNum=(SELECT COUNT(*) FROM blogcomment  WHERE ArticleId=ArticleId )
FROM blogarticle  WHERE IsPublished = 1  ORDER BY ViewNum DESC LIMIT @pageIndex, @pageSize) AS T1
LEFT JOIN blogtag AS bt ON bt.arId = T1.articleID
INNER JOIN blogcategory AS bc ON bc.categoryId = T1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result = await conn.QueryAsync<BlogArticle, BlogTag, BlogCategory, BlogArticle>(mysql, (ba, bt, bc) =>
                {
                    ba.Category = bc;

                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    //return bat;
                    ba.Tag = bt;
                    return ba;
                }, new { PageSize = pageSize, PageIndex = (pageIndex-1)*pageSize}, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public IEnumerable<BlogArticle> GetPagerByKeywords(int pageIndex, int pageSize, string condition = "")
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {              
                //string sql = @"SELECT t1.*,bt.*,bc.*
                //                FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId,paged.TitleImgPath,paged.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=paged.ArticleId )
                //                FROM (SELECT TOP  100 PERCENT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                //                FROM BlogArticle WHERE IsPublished=1 and body LIKE @Cond order by PostDate desc) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                //                LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                //                LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var mysql = @"SELECT T1.*,bt.*,bc.* 
FROM (SELECT ArticleId, Body, PostDate, Remark, ArticleName, IsPublished,CaId, TitleImgPath, ViewNum, CommentNum=(SELECT COUNT(*) FROM blogcomment  WHERE ArticleId=ArticleId )
FROM blogarticle  WHERE IsPublished = 1   AND body LIKE @Cond ORDER BY PostDate DESC LIMIT @pageIndex, @pageSize) AS T1
LEFT JOIN blogtag AS bt ON bt.arId = T1.articleID
INNER JOIN blogcategory AS bc ON bc.categoryId = T1.caId";
                var result = conn.Query<BlogArticle, BlogTag, BlogCategory, BlogArticle>(mysql, (ba, bt, bc) =>
                {
                    ba.Category = bc;

                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    //return bat;
                    ba.Tag = bt;
                    return ba;
                }, new { PageSize = pageSize, PageIndex = (pageIndex-1)*pageSize, Cond = "%" + condition + "%" }, splitOn: "ArticleId,TagId,CategoryId").AsQueryable();
                //return lookup.Values;
                return result;
            }
        }
        public async Task<IEnumerable<BlogArticle>> GetPagerByKeywordsAsync(int pageIndex, int pageSize, string condition = "")
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                //string sql = @"SELECT t1.*,bt.*,bc.*
                //            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId,paged.TitleImgPath,paged.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=paged.ArticleId )
                //            FROM (SELECT TOP  100 PERCENT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                //            FROM BlogArticle WHERE IsPublished=1 and body LIKE @Cond order by PostDate desc) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                //            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                //            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                var mysql = @"SELECT T1.*,bt.*,bc.* 
FROM (SELECT ArticleId, Body, PostDate, Remark, ArticleName, IsPublished,CaId, TitleImgPath, ViewNum, CommentNum=(SELECT COUNT(*) FROM blogcomment  WHERE ArticleId=ArticleId )
FROM blogarticle  WHERE IsPublished = 1   AND body LIKE @Cond ORDER BY PostDate DESC LIMIT @pageIndex, @pageSize) AS T1
LEFT JOIN blogtag AS bt ON bt.arId = T1.articleID
INNER JOIN blogcategory AS bc ON bc.categoryId = T1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result =await conn.QueryAsync<BlogArticle, BlogTag, BlogCategory, BlogArticle>(mysql, (ba, bt, bc) =>
                {
                    ba.Category = bc;

                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    //return bat;
                    ba.Tag = bt;
                    return ba;
                }, new { PageSize = pageSize, PageIndex = (pageIndex-1)*pageSize, Cond = "%" + condition + "%" }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public IEnumerable<BlogArticle> GetPagerByTag(int pageIndex, int pageSize, string tagName)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                //string sql = @"SELECT t1.*,bt.*,bc.*
                //            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId,paged.TitleImgPath,paged.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=paged.ArticleId )
                //            FROM (SELECT TOP 100 percent ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                //            FROM BlogArticle WHERE IsPublished=1 AND  articleId in(SELECT arId FROM blogTag WHERE tagName like @TagName) ORDER BY PostDate desc) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1 
                //            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                //            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                var mysql = @"SELECT T1.*,bt.*,bc.* 
FROM (SELECT ArticleId, Body, PostDate, Remark, ArticleName, IsPublished,CaId, TitleImgPath, ViewNum, CommentNum=(SELECT COUNT(*) FROM blogcomment  WHERE ArticleId=ArticleId )
FROM blogarticle  WHERE IsPublished = 1 AND  articleId IN (SELECT arId FROM blogtag WHERE tagName LIKE @TagName) ORDER BY PostDate DESC LIMIT @pageIndex, @pageSize) AS T1
LEFT JOIN blogtag AS bt ON bt.arId = T1.articleID
INNER JOIN blogcategory AS bc ON bc.categoryId = T1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result = conn.Query<BlogArticle, BlogTag, BlogCategory, BlogArticle>(mysql, (ba, bt, bc) =>
                {
                    ba.Category = bc;

                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    //return bat;
                    ba.Tag = bt;
                    return ba;
                }, new { PageSize = pageSize, PageIndex = (pageIndex - 1) * pageSize, TagName = $"%{tagName }%" }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public async Task<IEnumerable<BlogArticle>> GetPagerByTagAsync(int pageIndex, int pageSize, string tagName)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                //string sql = @"SELECT t1.*,bt.*,bc.*
                //            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId,paged.TitleImgPath,paged.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=paged.ArticleId )
                //            FROM (SELECT TOP 100 percent ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                //            FROM BlogArticle WHERE IsPublished=1 AND  articleId in(SELECT arId FROM blogTag WHERE tagName like @TagName) ORDER BY PostDate desc) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1 
                //            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                //            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                var mysql = @"SELECT T1.*,bt.*,bc.* 
FROM (SELECT ArticleId, Body, PostDate, Remark, ArticleName, IsPublished,CaId, TitleImgPath, ViewNum, CommentNum=(SELECT COUNT(*) FROM blogcomment  WHERE ArticleId=ArticleId )
FROM blogarticle  WHERE IsPublished = 1 AND  articleId IN (SELECT arId FROM blogtag WHERE tagName LIKE @TagName) ORDER BY PostDate DESC LIMIT @pageIndex, @pageSize) AS T1
LEFT JOIN blogtag AS bt ON bt.arId = T1.articleID
INNER JOIN blogcategory AS bc ON bc.categoryId = T1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result =await conn.QueryAsync<BlogArticle, BlogTag, BlogCategory, BlogArticle>(mysql, (ba, bt, bc) =>
                {
                    ba.Category = bc;

                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    //return bat;
                    ba.Tag = bt;
                    return ba;
                }, new { PageSize = pageSize, PageIndex = (pageIndex - 1) * pageSize, TagName = $"%{tagName }%" }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public IEnumerable<BlogArticle> GetPagerByCategory(int pageIndex, int pageSize, string categoryName)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                //string sql = @"SELECT t1.*,bt.*,bc.*
                //            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId,paged.TitleImgPath,paged.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=paged.ArticleId )
                //            FROM (SELECT top 100 Percent ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                //            FROM BlogArticle WHERE IsPublished=1 and CaId in (SELECT CategoryId FROM blogCategory WHERE CategoryName = @CategoryName) order by postDate desc) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                //            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                //            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                var mysql = @"SELECT T1.*,bt.*,bc.* 
FROM (SELECT ArticleId, Body, PostDate, Remark, ArticleName, IsPublished,CaId, TitleImgPath, ViewNum, CommentNum=(SELECT COUNT(*) FROM blogcomment  WHERE ArticleId=ArticleId )
FROM blogarticle  WHERE IsPublished = 1 AND  CaId IN (SELECT CategoryId FROM blogcategory WHERE CategoryName = @CategoryName) ORDER BY PostDate DESC LIMIT @pageIndex, @pageSize) AS T1
LEFT JOIN blogtag AS bt ON bt.arId = T1.articleID
INNER JOIN blogcategory AS bc ON bc.categoryId = T1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result = conn.Query<BlogArticle, BlogTag, BlogCategory, BlogArticle>(mysql, (ba, bt, bc) =>
                {
                    ba.Category = bc;

                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    //return bat;
                    ba.Tag = bt;
                    return ba;
                }, new { PageSize = pageSize, PageIndex = (pageIndex - 1) * pageSize, TagName = categoryName }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public async Task<IEnumerable<BlogArticle>> GetPagerByCategoryAsync(int pageIndex, int pageSize, string categoryName)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                
                //string sql = @"SELECT t1.*,bt.*,bc.*
                //            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId,paged.TitleImgPath,paged.ViewNum,CommentNum=(SELECT count(*) FROM BlogComment  WHERE ArticleId=paged.ArticleId )
                //            FROM (SELECT top 100 Percent ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                //            FROM BlogArticle WHERE IsPublished=1 and CaId in (SELECT CategoryId FROM blogCategory WHERE CategoryName = @CategoryName) order by postDate desc) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                //            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                //            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                var mysql = @"SELECT T1.*,bt.*,bc.* 
FROM (SELECT ArticleId, Body, PostDate, Remark, ArticleName, IsPublished,CaId, TitleImgPath, ViewNum, CommentNum=(SELECT COUNT(*) FROM blogcomment  WHERE ArticleId=ArticleId )
FROM blogarticle  WHERE IsPublished = 1 AND  CaId IN (SELECT CategoryId FROM blogcategory WHERE CategoryName = @CategoryName) ORDER BY PostDate DESC LIMIT @pageIndex, @pageSize) AS T1
LEFT JOIN blogtag AS bt ON bt.arId = T1.articleID
INNER JOIN blogcategory AS bc ON bc.categoryId = T1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result =await conn.QueryAsync<BlogArticle, BlogTag, BlogCategory, BlogArticle>(mysql, (ba, bt, bc) =>
                {
                    ba.Category = bc;

                    //if (!lookup.TryGetValue(ba.ArticleId, out BlogArticle bat))
                    //{
                    //    lookup.Add(ba.ArticleId, bat = ba);
                    //}
                    //ba.Tags = ba.Tags ?? new List<BlogTag>();
                    //bat.Tags.Add(bt);
                    //return bat;
                    ba.Tag = bt;
                    return ba;
                }, new { PageSize = pageSize, PageIndex = (pageIndex - 1) * pageSize, TagName = categoryName }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public (bool IsAdd, string msg) Add(BlogArticle blogArticle)
        {
            if (blogArticle == null)
            {
                return(false, "blogArticle不能为null" );
            }
            else if (!string.IsNullOrEmpty(blogArticle.ArticleName)&& !string.IsNullOrEmpty(blogArticle.Body) && blogArticle.Category !=null&& (GetExistCount("select count(*) from blogcategory where categoryId=@caId", new { caId = blogArticle.Category.CategoryId }) > 0))
            {
                var m = GetExistCount("select count(*) from blogarticle where ArticleName=@artname", new { artname = blogArticle.ArticleName });
                if (GetExistCount("select count(*) from blogarticle where ArticleName=@artname", new { artname = blogArticle.ArticleName })>0)
                {
                    return (false, "文章标题重复" );
                }
                using (var conn=ConnectionFactory.GetOpenConnection())
                {
                    
                    var tran = conn.BeginTransaction();
                    try
                    {
                        //var currentId = GetMaxId()+1;
                        //&&GetExistCount("select count(*) from blogtag where TagName like @atnames and ArId=@articleId", new { articleId = GetMaxId(), atnames = $"%{blogArticle.Tag.TagName}%" })<=0
                        var articleNum = conn.Execute("insert into blogarticle(CaId,Body,PostDate,Remark,ArticleName,IsPublished,TitleImgPath) values(@caid,@body,@postDate,@remark,@articleName,@isPublished,@titleImgPath)", new { caid=blogArticle.Category.CategoryId,body=blogArticle.Body,postDate=blogArticle.PostDate,remark=blogArticle.Remark,articleName=blogArticle.ArticleName,isPublished=blogArticle.IsPublished, titleImgPath=blogArticle.TitleImgPath}, tran);
                        if (blogArticle.Tag != null&&!string.IsNullOrEmpty(blogArticle.Tag.TagName))
                        { 
                            var tagNum = conn.Execute("insert into blogtag(ArId,TagName) values(LAST_INSERT_ID(),@tagName)", new {tagName=blogArticle.Tag.TagName }, tran);
                        
                            if (tagNum==0)
                            {
                                tran.Rollback();
                                return (false,"标签保存失败");
                            }
                        }
                        tran.Commit();
                        if (articleNum == 0)
                        {
                            return (false, "文章保存失败");
                        }
                        return (true, "文章保存成功");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false,  ex.Message );
                    }
                }
            }
            else
            {
                return (false, "blogArticle属性值不符");
            }
        }
        public async Task<(bool IsAdd, string msg)> AddAsync(BlogArticle blogArticle)
        {
            if (blogArticle == null)
            {
                return (false, "blogArticle不能为null" );
            }
            else if (!string.IsNullOrEmpty(blogArticle.ArticleName) && !string.IsNullOrEmpty(blogArticle.Body) && blogArticle.Category != null && (GetExistCount("select count(*) from blogcategory where categoryId=@caId", new { caId = blogArticle.Category.CategoryId }) > 0))
            {
                var m = GetExistCount("select count(*) from blogarticle where ArticleName=@artname", new { artname = blogArticle.ArticleName });
                if (GetExistCount("select count(*) from blogarticle where ArticleName=@artname", new { artname = blogArticle.ArticleName }) > 0)
                {
                    return (false,  "文章标题重复" );
                }
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    
                    var tran = conn.BeginTransaction();
                    try
                    {
                        //var currentId = GetMaxId() + 1;
                        var articleNum =await conn.ExecuteAsync("insert into blogarticle(CaId,Body,PostDate,Remark,ArticleName,IsPublished,TitleImgPath) values(@caid,@body,@postDate,@remark,@articleName,@isPublished,@titleImgPath)", new { caid = blogArticle.Category.CategoryId, body = blogArticle.Body, postDate = blogArticle.PostDate, remark = blogArticle.Remark, articleName = blogArticle.ArticleName, isPublished = blogArticle.IsPublished, titleImgPath=blogArticle.TitleImgPath }, tran);
                        if (blogArticle.Tag != null && !string.IsNullOrEmpty(blogArticle.Tag.TagName))
                        {
                            var tagNum =await conn.ExecuteAsync("insert into blogtag(ArId,TagName) values(LAST_INSERT_ID(),@tagName)", new {tagName = blogArticle.Tag.TagName }, tran);

                            if (tagNum == 0)
                            {
                                tran.Rollback();
                                return (false,"标签保存失败");
                            }
                        }
                        tran.Commit();
                        if (articleNum == 0)
                        {
                            return ( false, "文章保存失败");
                        }
                        return (true, "文章保存成功");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false, ex.Message);
                    }
                }
            }
            else
            {
                return (false,  "blogArticle属性值不符");
            }
        }
        public (bool IsDelete, string msg) Delete(BlogArticle blogArticle)
        {
            if (blogArticle == null)
            {
                return (false,"blogArticle不能为null");
            }
            if (GetExistCount("select count(*) from blogarticle where articleId=@arId",new { arId=blogArticle.ArticleId})==0)
            {
                return (false, "异常！不存在的blogArticle");
            }
            else
            {
                using (var conn=ConnectionFactory.GetOpenConnection())
                {
                    
                    var tran = conn.BeginTransaction();
                    var obj = new { IsDelete = false, msg = "删除失败" };
                    try
                    {
                        if (blogArticle.Tag!=null)
                        {
                            if (GetExistCount("select count(*) from blogtag where Arid=@arId",new { arId=blogArticle.ArticleId})>0)
                            {
                               var tgNum= conn.Execute("delete from blogtag where ArId=@artId;",new { artId=blogArticle.ArticleId},tran);
                                if (tgNum<=0)
                                {
                                    tran.Rollback();
                                    return (false, "标签删除失败，回滚");
                                }
                            }

                            //var result= conn.QueryMultiple("delete from blogTag where ArId=@arId;delete from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
                            // if (result.Read<int>().ToList().Where(t=>t>0).Count()==2)
                            // {
                            //     tran.Commit();
                            //     return (true, "删除成功" );
                            // }
                            // else
                            // {
                            //     tran.Rollback();
                            //     return (false,"删除失败");
                            // }
                            var result = conn.Execute("delete from blogarticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
                            if (result>0)
                            {
                                tran.Commit();
                                return (true, "删除成功");
                            }
                            else
                            {
                                tran.Rollback();
                                return (false, "删除失败");
                            }
                        }
                        else
                        {
                            var result = conn.Execute("delete from blogarticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
                            if (result>0)
                            {
                                tran.Commit();
                                return (true,  "删除成功");
                            }
                            else
                            {
                                tran.Rollback();
                                return (false, "删除失败" );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false, ex.Message);
                    }
                    
                }
            }
        }
        public async Task<(bool IsDelete, string msg)> DeleteAsync(BlogArticle blogArticle)
        {
            if (blogArticle == null)
            {
                return (false,  "blogArticle不能为null" );
            }
            if (GetExistCount("select count(*) from blogarticle where articleId=@arId", new { arId = blogArticle.ArticleId }) == 0)
            {
                return (false,  "异常！不存在的blogArticle");
            }
            else
            {
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    
                    var tran = conn.BeginTransaction();
                    try
                    {
                        if (blogArticle.Tag != null)
                        {
                            if (GetExistCount("select count(*) from blogtag where Arid=@arId", new { arId = blogArticle.ArticleId }) > 0)
                            {
                                var tgNum =await conn.ExecuteAsync("delete from blogtag where ArId=@arId;", new { arId = blogArticle.ArticleId });
                                if (tgNum <= 0)
                                {
                                    return (true, "标签删除失败，回滚");
                                }
                            }

                            //var result =await conn.QueryMultipleAsync("delete from blogTag where ArId=@arId;delete from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
                            //if (result.Read<int>().ToList().Where(t => t > 0).Count() == 2)
                            //{
                            //    tran.Commit();
                            //    return (true,"删除成功");
                            //}
                            //else
                            //{
                            //    tran.Rollback();
                            //    return (false, "删除失败");
                            //}
                            var result =await conn.ExecuteAsync("delete from blogarticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
                            if (result > 0)
                            {
                                tran.Commit();
                                return (true, "删除成功");
                            }
                            else
                            {
                                tran.Rollback();
                                return (false, "删除失败");
                            }
                        }
                        else
                        {
                            var result =await conn.ExecuteAsync("delete from blogarticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
                            if (result > 0)
                            {
                                tran.Commit();
                                return (true,"删除成功");
                            }
                            else
                            {
                                tran.Rollback();
                                return (false, "删除失败");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false, ex.Message);
                    }

                }
            }
        }
        public (bool IsUpdate,string msg) Update(BlogArticle blogArticle)
        {           
            if (blogArticle == null)
            {
                return (false,  "blogArticle不能为null");
            }
            if (GetExistCount("select count(*) from blogarticle where articleId=@arId", new { arId = blogArticle.ArticleId }) == 0)
            {
                return (false,  "异常！不存在的blogArticle" );
            }
            else
            {
                using (var conn=ConnectionFactory.GetOpenConnection())
                {
                    
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var ArtUpNum = conn.Execute("update blogarticle set articleName=@artName,body=@body,caId=@caId,postDate=@postDate,remark=@remark,ispublished=@ispublished,TitleImgPath=@titleImgPath,ViewNum=@viewNum where articleId=@arId", new { artName = blogArticle.ArticleName, body = blogArticle.Body, caId = blogArticle.Category.CategoryId, postDate = blogArticle.PostDate, remark = blogArticle.Remark, ispublished = blogArticle.IsPublished, arId = blogArticle.ArticleId, titleImgPath=blogArticle.TitleImgPath,viewNum=blogArticle.ViewNum },tran);

                        var tgUp = CheckTagUpdate();
                        bool CheckTagUpdate()
                        {
                            var TagUpNum = 0;
                            var TagDelNum = 0;
                            if (blogArticle.Tag != null)
                            {
                                if (blogArticle.Tag.TagName.Trim()=="")
                                {
                                    TagDelNum= conn.Execute("delete from blogtag where TagId=@tId", new { tId = blogArticle.Tag.TagId }, tran);
                                }
                                else
                                {
                                    TagUpNum= conn.Execute("update blogtag set tagName=@tgName where TagId=@tId", new { tgName = blogArticle.Tag.TagName, tId = blogArticle.Tag.TagId }, tran);
                                }
                                return (TagUpNum > 0 || TagDelNum > 0) ? true : false;
                            }
                            //else
                            //{
                            //    TagDelNum= conn.Execute("delete from blogtag where TagId=@tId", new { tId = blogArticle.Tag.TagId }, tran);
                            //}
                            return true;
                        }  //检测Tag是否需要更新
                        if (ArtUpNum > 0 && tgUp)
                        {
                            tran.Commit();
                            return (true, "修改成功");
                        }
                        else
                        {
                            tran.Rollback();
                            return (false, "修改失败" );
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false,  ex.Message );
                    }
                }
            }
        }
        public async Task<(bool IsUpdate, string msg)> UpdateAsync(BlogArticle blogArticle)
        {

            if (blogArticle == null)
            {
                return (false,  "blogArticle不能为null" );
            }
            if (GetExistCount("select count(*) from blogarticle where articleId=@arId", new { arId = blogArticle.ArticleId }) == 0)
            {
                return (false, "异常！不存在的blogArticle");
            }
            else
            {
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var ArtUpNum =await conn.ExecuteAsync("update blogarticle set articleName=@artName,body=@body,caId=@caId,postDate=@postDate,remark=@remark,ispublished=@ispublished,TitleImgPath=@titleImgPath,ViewNum=@viewNum where articleId=@arId", new { artName = blogArticle.ArticleName, body = blogArticle.Body, caId = blogArticle.Category.CategoryId, postDate = blogArticle.PostDate, remark = blogArticle.Remark, ispublished = blogArticle.IsPublished, arId = blogArticle.ArticleId, titleImgPath=blogArticle.TitleImgPath, viewNum = blogArticle.ViewNum }, tran);

                        var tgUp =await CheckTagUpdateAsync();
                       async Task<bool> CheckTagUpdateAsync()
                        {
                            var TagUpNum = 0;
                            var TagDelNum = 0;
                            if (blogArticle.Tag != null)
                            {
                                if (blogArticle.Tag.TagName.Trim() == "")
                                {
                                    TagDelNum =await conn.ExecuteAsync("delete from blogtag where TagId=@tId", new { tId = blogArticle.Tag.TagId }, tran);
                                }
                                else
                                {
                                    TagUpNum =await conn.ExecuteAsync("update blogtag set tagName=@tgName where TagId=@tId", new { tgName = blogArticle.Tag.TagName, tId = blogArticle.Tag.TagId }, tran);
                                }
                                return (TagUpNum > 0 || TagDelNum > 0) ? true : false;
                            }
                            //else
                            //{
                            //    TagDelNum = await conn.ExecuteAsync("delete from blogtag where TagId=@tId", new { tId = blogArticle.Tag.TagId }, tran);
                            //}
                            return true;
                        }  //检测Tag是否需要更新
                        if (ArtUpNum > 0 && tgUp)
                        {
                            tran.Commit();
                            return (true,"修改成功");
                        }
                        else
                        {
                            tran.Rollback();
                            return (false, "修改失败" );
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false,  ex.Message );
                    }
                }
            }
        }
    }
}

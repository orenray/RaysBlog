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
            return GetExistCount("select count(*) from blogArticle where ispublished=1",null);
        }
        public int GetMaxId()
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return conn.ExecuteScalar<int>("select max(articleId) from blogArticle where ispublished=1");
            }
        }
        public BlogArticle Get(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT ba.*,bt.*,bc.*
                                FROM BlogArticle AS ba INNER JOIN BlogTag AS bt ON bt.ArId=ba.ArticleId 
                                INNER JOIN BlogCategory AS bc ON ba.CaId=bc.CategoryId
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
                string sql = @"SELECT ba.*,bt.*,bc.*
                                FROM BlogArticle AS ba INNER JOIN BlogTag AS bt ON bt.ArId=ba.ArticleId 
                                INNER JOIN BlogCategory AS bc ON ba.CaId=bc.CategoryId
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
        public IEnumerable<BlogArticle> GetPagerByKeywords(int pageIndex, int pageSize, string condition = "")
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT t1.*,bt.*,bc.*
                            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId
                            FROM (SELECT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                            FROM BlogArticle WHERE body LIKE '%%') AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result = conn.Query<BlogArticle, BlogTag, BlogCategory, BlogArticle>(sql, (ba, bt, bc) =>
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
                }, new { PageSize = pageSize, PageIndex = pageIndex, Cond = "%" + condition + "%" }, splitOn: "ArticleId,TagId,CategoryId").AsQueryable();
                //return lookup.Values;
                return result;
            }
        }
        public async Task<IEnumerable<BlogArticle>> GetPagerByKeywordsAsync(int pageIndex, int pageSize, string condition = "")
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT t1.*,bt.*,bc.*
                            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId
                            FROM (SELECT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                            FROM BlogArticle WHERE body LIKE '%%') AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result =await conn.QueryAsync<BlogArticle, BlogTag, BlogCategory, BlogArticle>(sql, (ba, bt, bc) =>
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
                }, new { PageSize = pageSize, PageIndex = pageIndex, Cond = "%" + condition + "%" }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public IEnumerable<BlogArticle> GetPagerByTag(int pageIndex, int pageSize, string tagName)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT t1.*,bt.*,bc.*
                            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId
                            FROM (SELECT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                            FROM BlogArticle WHERE articleId in(SELECT arId FROM blogTag WHERE tagName like @TagName)) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result = conn.Query<BlogArticle, BlogTag, BlogCategory, BlogArticle>(sql, (ba, bt, bc) =>
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
                }, new { PageSize = pageSize, PageIndex = pageIndex, TagName = $"%{tagName }%" }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public async Task<IEnumerable<BlogArticle>> GetPagerByTagAsync(int pageIndex, int pageSize, string tagName)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT t1.*,bt.*,bc.*
                            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId
                            FROM (SELECT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                            FROM BlogArticle WHERE articleId in(SELECT arId FROM blogTag WHERE tagName like @TagName)) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result =await conn.QueryAsync<BlogArticle, BlogTag, BlogCategory, BlogArticle>(sql, (ba, bt, bc) =>
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
                }, new { PageSize = pageSize, PageIndex = pageIndex, TagName = $"%{tagName }%" }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public IEnumerable<BlogArticle> GetPagerByCategory(int pageIndex, int pageSize, string categoryName)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT t1.*,bt.*,bc.*
                            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId
                            FROM (SELECT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                            FROM BlogArticle WHERE CaId = (SELECT CategoryId FROM blogCategory WHERE CategoryName = @CategoryName)) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result = conn.Query<BlogArticle, BlogTag, BlogCategory, BlogArticle>(sql, (ba, bt, bc) =>
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
                }, new { PageSize = pageSize, PageIndex = pageIndex, TagName = categoryName }, splitOn: "ArticleId,TagId,CategoryId");
                //return lookup.Values;
                return result.AsQueryable();
            }
        }
        public async Task<IEnumerable<BlogArticle>> GetPagerByCategoryAsync(int pageIndex, int pageSize, string categoryName)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                string sql = @"SELECT t1.*,bt.*,bc.*
                            FROM (SELECT TOP (@PageSize) paged.ArticleId,paged.Body,paged.PostDate,paged.Remark,paged.ArticleName,paged.IsPublished,paged.CaId
                            FROM (SELECT ROW_NUMBER() OVER (ORDER BY articleId) AS [No],*
                            FROM BlogArticle WHERE CaId = (SELECT CategoryId FROM blogCategory WHERE CategoryName = @CategoryName)) AS Paged WHERE [No] > (@PageIndex - 1) * @PageSize) AS T1
                            LEFT JOIN Blogtag AS bt ON bt.arId=T1.articleID
                            LEFT JOIN BlogCategory AS bc ON bc.categoryId =t1.caId";
                //var lookup = new Dictionary<int, BlogArticle>();
                var result =await conn.QueryAsync<BlogArticle, BlogTag, BlogCategory, BlogArticle>(sql, (ba, bt, bc) =>
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
                }, new { PageSize = pageSize, PageIndex = pageIndex, TagName = categoryName }, splitOn: "ArticleId,TagId,CategoryId");
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
            else if (!string.IsNullOrEmpty(blogArticle.ArticleName)&& !string.IsNullOrEmpty(blogArticle.Body) && blogArticle.Category !=null&& (GetExistCount("select count(*) from BlogCategory where categoryId=@caId", new { caId = blogArticle.Category.CategoryId }) > 0))
            {
                var m = GetExistCount("select count(*) from BlogArticle where ArticleName=@artname", new { artname = blogArticle.ArticleName });
                if (GetExistCount("select count(*) from BlogArticle where ArticleName=@artname", new { artname = blogArticle.ArticleName })>0)
                {
                    return (false, "文章标题重复" );
                }
                using (var conn=ConnectionFactory.GetOpenConnection())
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var currentId = GetMaxId()+1;
                        var articleNum= conn.Execute("set IDENTITY_INSERT blogArticle  on;insert into blogArticle(ArticleId,CaId,Body,PostDate,Remark,ArticleName,IsPublished) values(@articleId,@caid,@body,@postDate,@remark,@articleName,@isPublished);set IDENTITY_INSERT blogArticle  off", new {articleId= currentId, caid=blogArticle.Category.CategoryId,body=blogArticle.Body,postDate=blogArticle.PostDate,remark=blogArticle.Remark,articleName=blogArticle.ArticleName,isPublished=blogArticle.IsPublished }, tran);
                        if (blogArticle.Tag != null&&GetExistCount("select count(*) from BlogTag where TagName like @atnames and ArId=@articleId", new { articleId = blogArticle.ArticleId, atnames = $"%{blogArticle.Tag.TagName}%" })<=0)
                        { 
                            var tagNum = conn.Execute("insert into blogTag(ArId,TagName) values(@arID,@tagName)", new {arID= currentId, tagName=blogArticle.Tag.TagName }, tran);
                        
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
            else if (!string.IsNullOrEmpty(blogArticle.ArticleName) && !string.IsNullOrEmpty(blogArticle.Body) && blogArticle.Category != null && (GetExistCount("select count(*) from BlogCategory where categoryId=@caId", new { caId = blogArticle.Category.CategoryId }) > 0))
            {
                var m = GetExistCount("select count(*) from BlogArticle where ArticleName=@artname", new { artname = blogArticle.ArticleName });
                if (GetExistCount("select count(*) from BlogArticle where ArticleName=@artname", new { artname = blogArticle.ArticleName }) > 0)
                {
                    return (false,  "文章标题重复" );
                }
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var currentId = GetMaxId() + 1;
                        var articleNum =await conn.ExecuteAsync("insert into blogArticle(ArticleId,CaId,Body,PostDate,Remark,ArticleName,IsPublished) values(@articleId,@caid,@body,@postDate,@remark,@articleName,@isPublished)", new { articleId = currentId, caid = blogArticle.Category.CategoryId, body = blogArticle.Body, postDate = blogArticle.PostDate, remark = blogArticle.Remark, articleName = blogArticle.ArticleName, isPublished = blogArticle.IsPublished }, tran);
                        if (blogArticle.Tag != null && GetExistCount("select count(*) from BlogTag where TagName like @atnames and ArId=@articleId", new { articleId = currentId, atnames = $"%{blogArticle.Tag.TagName}%" }) <= 0)
                        {
                            var tagNum =await conn.ExecuteAsync("insert into blogTag(ArId,TagName) values(@arID,@tagName)", new { arID = blogArticle.ArticleId, tagName = blogArticle.Tag.TagName }, tran);

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
            if (GetExistCount("select count(*) from blogArticle where articleId=@arId",new { arId=blogArticle.ArticleId})==0)
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
                                conn.Execute("delete from blogTag where ArId=@arId;");
                            }
                        
                           var result= conn.QueryMultiple("delete from blogTag where ArId=@arId;delete from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
                            if (result.Read<int>().ToList().Where(t=>t>0).Count()==2)
                            {
                                tran.Commit();
                                return (true, "删除成功" );
                            }
                            else
                            {
                                tran.Rollback();
                                return (false,"删除失败");
                            }
                        }
                        else
                        {
                            var result = conn.Execute("delete from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
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
                        return (true, ex.Message);
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
            if (GetExistCount("select count(*) from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }) == 0)
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
                                conn.Execute("delete from blogTag where ArId=@arId;");
                            }

                            var result =await conn.QueryMultipleAsync("delete from blogTag where ArId=@arId;delete from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
                            if (result.Read<int>().ToList().Where(t => t > 0).Count() == 2)
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
                        else
                        {
                            var result =await conn.ExecuteAsync("delete from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }, tran);
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
                        return (true, ex.Message);
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
            if (GetExistCount("select count(*) from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }) == 0)
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
                        var ArtUpNum = conn.Execute("update blogArticle set articleName=@artName,body=@body,caId=@caId,postDate=@postDate,remark=@remark,ispublished=@ispublished where articleId=@arId", new { artName = blogArticle.ArticleName, body = blogArticle.Body, caId = blogArticle.Category.CategoryId, postDate = blogArticle.PostDate, remark = blogArticle.Remark, ispublished = blogArticle.IsPublished, arId = blogArticle.ArticleId },tran);

                        var tgUp = CheckTagUpdate();
                        bool CheckTagUpdate()
                        {
                            var TagUpNum = 0;
                            var TagDelNum = 0;
                            if (blogArticle.Tag != null)
                            {
                                if (blogArticle.Tag.TagName.Trim()=="")
                                {
                                    TagDelNum= conn.Execute("delete from blogTag where TagId=@tId", new { tId = blogArticle.Tag.TagId }, tran);
                                }
                                else
                                {
                                    TagUpNum= conn.Execute("update blogTag set tagName=@tgName where TagId=@tId", new { tgName = blogArticle.Tag.TagName, tId = blogArticle.Tag.TagId }, tran);
                                }
                            }
                            else
                            {
                                TagDelNum= conn.Execute("delete from blogTag where TagId=@tId", new { tId = blogArticle.Tag.TagId }, tran);
                            }
                            return (TagUpNum > 0 || TagDelNum > 0) ? true : false;
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
            if (GetExistCount("select count(*) from blogArticle where articleId=@arId", new { arId = blogArticle.ArticleId }) == 0)
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
                        var ArtUpNum =await conn.ExecuteAsync("update blogArticle set articleName=@artName,body=@body,caId=@caId,postDate=@postDate,remark=@remark,ispublished=@ispublished where articleId=@arId", new { artName = blogArticle.ArticleName, body = blogArticle.Body, caId = blogArticle.Category.CategoryId, postDate = blogArticle.PostDate, remark = blogArticle.Remark, ispublished = blogArticle.IsPublished, arId = blogArticle.ArticleId }, tran);

                        var tgUp =await CheckTagUpdateAsync();
                       async Task<bool> CheckTagUpdateAsync()
                        {
                            var TagUpNum = 0;
                            var TagDelNum = 0;
                            if (blogArticle.Tag != null)
                            {
                                if (blogArticle.Tag.TagName.Trim() == "")
                                {
                                    TagDelNum =await conn.ExecuteAsync("delete from blogTag where TagId=@tId", new { tId = blogArticle.Tag.TagId }, tran);
                                }
                                else
                                {
                                    TagUpNum =await conn.ExecuteAsync("update blogTag set tagName=@tgName where TagId=@tId", new { tgName = blogArticle.Tag.TagName, tId = blogArticle.Tag.TagId }, tran);
                                }
                            }
                            else
                            {
                                TagDelNum =await conn.ExecuteAsync("delete from blogTag where TagId=@tId", new { tId = blogArticle.Tag.TagId }, tran);
                            }
                            return (TagUpNum > 0 || TagDelNum > 0) ? true : false;
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

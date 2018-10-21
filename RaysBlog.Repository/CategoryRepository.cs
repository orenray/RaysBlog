//using DapperExtensions;
using Dapper;
using RaysBlog.Model;
using RaysBlog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaysBlog.Repository
{
    public class CategoryRepository// : BaseRepository<BlogCategory>, ICategoryRepository
    {
        //public override IEnumerable<BlogCategory> GetEntities(int pageIndex, int pageSize, bool ascending = true)
        //{
        //    if (pageIndex < 1) pageIndex = 1;
        //    if (pageSize > 10) pageSize = 10;
        //    using (var conn = ConnectionFactory.GetOpenConnection())
        //    {
        //        var count = conn.Count<BlogCategory>(null);

        //        var pages = conn.GetPage<BlogCategory>(null, new List<ISort> { Predicates.Sort<BlogCategory>(s => s.Id, ascending) }, pageIndex - 1, pageSize).ToList();
        //        return pages;
        //    }
        //}
        private int GetExistCount(string sql, object parameter)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var result = conn.ExecuteScalar<int>(sql, parameter);
                return result;
            }
        }
        public int GetTotalCount()
        {
            return GetExistCount("select count(*) from blogcategory", null);
        }
        public BlogCategory Get(int id)
        {
            using (var conn=ConnectionFactory.GetOpenConnection())
            {
               return conn.QueryFirstOrDefault<BlogCategory>("select * from blogcategory where categoryId=@caId",new {caId=id });
            }
        }
        public async Task<BlogCategory> GetAsync(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<BlogCategory>("select * from blogcategory where categoryId=@caId", new { caId = id });
            }
        }
        public IEnumerable<BlogCategory> GetCategorys()
        {
            using (var conn=ConnectionFactory.GetOpenConnection())
            {
               return conn.Query<BlogCategory>("select * from blogcategory order by categoryId");
            }
        }
        public async Task<IEnumerable<BlogCategory>> GetCategorysAsync()
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return await conn.QueryAsync<BlogCategory>("select * from blogcategory order by categoryId");
            }
        }
        public IEnumerable<BlogCategory> GetPager(int pageIndex,int pageSize)
        {
            using (var conn=ConnectionFactory.GetOpenConnection())
            {
                //var sql = @"SELECT top(@PageSize) paged.categoryId,paged.CategoryName FROM (SELECT row_number() over(ORDER BY categoryId) AS [No],* FROM blogcategory ) AS paged WHERE paged.[No]>(@PageIndex-1)*@PageSize";
                var mysql = @"SELECT * FROM blogcategory order by categoryId LIMIT @pageIndex, @pageSize";
                return conn.Query<BlogCategory>(mysql,new { PageIndex=(pageIndex-1)*pageSize,PageSize=pageSize});
            }
        }
        public async Task<IEnumerable<BlogCategory>> GetPagerAsync(int pageIndex, int pageSize)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                //var sql = @"SELECT top(@PageSize) paged.categoryId,paged.CategoryName FROM (SELECT row_number() over(ORDER BY categoryId) AS [No],* FROM blogcategory ) AS paged WHERE paged.[No]>(@PageIndex-1)*@PageSize";
                var mysql = @"SELECT * FROM blogcategory order by categoryId LIMIT @pageIndex, @pageSize";
                return await conn.QueryAsync<BlogCategory>(mysql, new { PageIndex = (pageIndex - 1) * pageSize, PageSize = pageSize });
            }
        }
        public (bool IsAdd, string msg) Add(BlogCategory category)
        {
            if (category==null)
            {
                return (false,"category为null");
            }
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                return (false, "categoryName为空或null");
            }
            if (GetExistCount("select count(*) from blogcategory where categoryName=@caName", new {caName=category.CategoryName })>0)
            {
                return (false, "分类已经存在" );
            }
            else
            {
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var num = conn.Execute("Insert into blogcategory(CategoryName) Values(@caName)", new { caName = category.CategoryName },tran);
                        if (num>0)
                        {
                            tran.Commit();
                            return (true, "分类保存成功");
                        }
                        else
                        {
                            tran.Rollback();
                            return (false, "分类保存失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false, ex.Message );
                    }
                }
            }
        }
        public async Task<(bool IsAdd, string msg)> AddAsync(BlogCategory category)
        {
            if (category == null)
            {
                return (false, "category为null");
            }
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                return (false, "categoryName为空或null");
            }
            if (GetExistCount("select count(*) from blogcategory where categoryName=@caName", new { caName = category.CategoryName }) > 0)
            {
                return (false,"分类已经存在");
            }
            else
            {
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var num =await conn.ExecuteAsync("Insert into blogcategory(CategoryName) Values(@caName)", new { caName = category.CategoryName }, tran);
                        if (num > 0)
                        {
                            tran.Commit();
                            return (true,"分类保存成功");
                        }
                        else
                        {
                            tran.Rollback();
                            return (false,"分类保存失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false, ex.Message );
                    }
                }
            }
        }
        public (bool IsDelete, string msg) Delete(BlogCategory category)
        {
            if (category == null)
            {
                return (false,"category为null");
            }
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                return (false,"categoryName为空或null");
            }
            if (GetExistCount("select count(*) from blogcategory where categoryName=@caName", new { caName = category.CategoryName }) <= 0)
            {
                return (false,"分类不存在");
            }
            else
            {
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var num = conn.Execute("delete from blogcategory where CategoryId=@caId", new { caId = category.CategoryId }, tran);
                        if (num > 0)
                        {
                            tran.Commit();
                            return (true,"分类删除成功");
                        }
                        else
                        {
                            tran.Rollback();
                            return (false,"分类删除失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false,ex.Message);
                    }
                }
            }
        }
        public async Task<(bool IsDelete, string msg)> DeleteAsync(BlogCategory category)
        {
            if (category == null)
            {
                return (false,  "category为null" );
            }
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                return (false, "categoryName为空或null");
            }
            if (GetExistCount("select count(*) from blogcategory where categoryName=@caName", new { caName = category.CategoryName }) <= 0)
            {
                return (false, "分类不存在");
            }
            else
            {
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var num =await conn.ExecuteAsync("delete from blogcategory where CategoryId=@caId", new { caId = category.CategoryId }, tran);
                        if (num > 0)
                        {
                            tran.Commit();
                            return (true, "分类删除成功" );
                        }
                        else
                        {
                            tran.Rollback();
                            return (false, "分类删除失败");
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
        public (bool IsUpdate, string msg) Update(BlogCategory category)
        {
            if (category == null)
            {
                return (false,  "category为null");
            }
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                return (false,"categoryName为空或null");
            }
            if (GetExistCount("select count(*) from blogcategory where categoryId=@caId", new { caId = category.CategoryId }) <= 0)
            {
                return (false,"分类不存在");
            }
            else
            {
                using (var conn = ConnectionFactory.GetOpenConnection())
                {

                    var tran = conn.BeginTransaction();
                    try
                    {
                        var num = conn.Execute("update blogcategory set categoryName=@caName where CategoryId=@caId", new { caName=category.CategoryName,caId = category.CategoryId }, tran);
                        if (num > 0)
                        {
                            tran.Commit();
                            return (true, "分类更新成功");
                        }
                        else
                        {
                            tran.Rollback();
                            return (false,"分类更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false,ex.Message);
                    }
                }
            }
        }
        public async Task<(bool IsUpdate, string msg)> UpdateAsync(BlogCategory category)
        {
            if (category == null)
            {
                return (false, "category为null");
            }
            if (string.IsNullOrEmpty(category.CategoryName))
            {
                return (false,"categoryName为空或null");
            }
            if (GetExistCount("select count(*) from blogcategory where categoryId=@caId", new { caId = category.CategoryId }) <= 0)
            {
                return (false,"分类不存在");
            }
            else
            {
                using (var conn = ConnectionFactory.GetOpenConnection())
                {
                    var tran = conn.BeginTransaction();
                    try
                    {
                        var num =await conn.ExecuteAsync("update blogcategory set categoryName=@caName where CategoryId=@caId", new { caName = category.CategoryName, caId = category.CategoryId }, tran);
                        if (num > 0)
                        {
                            tran.Commit();
                            return (true,"分类更新成功");
                        }
                        else
                        {
                            tran.Rollback();
                            return (false,"分类更新失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        return (false,ex.Message);
                    }
                }
            }
        }
    }
}

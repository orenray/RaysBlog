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
    public class TagRepository //: BaseRepository<BlogTag>, ITagRepository
    {
        //public override IEnumerable<BlogTag> GetEntities(int pageIndex, int pageSize, bool ascending=true)
        //{
        //    if (pageIndex < 1) pageIndex = 1;
        //    if (pageSize > 10) pageSize = 10;
        //    using (var conn = ConnectionFactory.GetOpenConnection())
        //    {
        //        var count = conn.Count<BlogTag>(null);

        //        var pages = conn.GetPage<BlogTag>(null, new List<ISort> { Predicates.Sort<BlogTag>(s => s.Id, ascending) }, pageIndex - 1, pageSize);             
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
            return GetExistCount("select count(*) from blogtag", null);
        }
        public BlogTag Get(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return conn.QueryFirstOrDefault<BlogTag>("select * from blogtag where TagId=@tgId", new { tgId = id });
            }
        }
        public async Task<BlogTag> GetAsync(int id)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<BlogTag>("select * from blogtag where TagId=@tgId", new { tgId = id });
            }
        }
        public IEnumerable<BlogTag> GetPager(int pageIndex, int pageSize) 
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var sql = @"SELECT top(@PageSize) paged.TagId,paged.ArId,paged.TagName FROM (SELECT row_number() over(ORDER BY TagId) AS [No],* FROM BlogTag ) AS paged WHERE paged.[No]>(@PageIndex-1)*@PageSize";
                var mysql = @"SELECT * FROM blogtag order by tagId LIMIT @pageIndex, @pageSize";
                return conn.Query<BlogTag>(mysql, new { PageIndex = (pageIndex-1)*pageSize, PageSize = pageSize });
            }
        }
        public async Task<IEnumerable<BlogTag>> GetPagerAsync(int pageIndex, int pageSize)
        {
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var sql = @"SELECT top(@PageSize) paged.TagId,paged.ArId,paged.TagName FROM (SELECT row_number() over(ORDER BY TagId) AS [No],* FROM BlogTag ) AS paged WHERE paged.[No]>(@PageIndex-1)*@PageSize";
                var mysql = @"SELECT * FROM blogtag order by tagId LIMIT @pageIndex, @pageSize";
                return await conn.QueryAsync<BlogTag>(mysql, new { PageIndex = (pageIndex - 1) * pageSize, PageSize = pageSize });
            }
        }
    }
}

using DapperExtensions;
using RaysBlog.Model;
using RaysBlog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaysBlog.Repository
{
    public class CategoryRepository : BaseRepository<BlogCategory>, ICategoryRepository
    {
        public override IEnumerable<BlogCategory> GetEntities(int pageIndex, int pageSize, bool ascending = true)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize > 10) pageSize = 10;
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var count = conn.Count<BlogCategory>(null);

                var pages = conn.GetPage<BlogCategory>(null, new List<ISort> { Predicates.Sort<BlogCategory>(s => s.Id, ascending) }, pageIndex - 1, pageSize).ToList();
                return pages;
            }
        }
    }
}

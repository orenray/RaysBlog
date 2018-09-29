using DapperExtensions;
using RaysBlog.Model;
using RaysBlog.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RaysBlog.Repository
{
    public class TagRepository : BaseRepository<BlogTag>, ITagRepository
    {
        public override IEnumerable<BlogTag> GetEntities(int pageIndex, int pageSize, bool ascending=true)
        {
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize > 10) pageSize = 10;
            using (var conn = ConnectionFactory.GetOpenConnection())
            {
                var count = conn.Count<BlogTag>(null);

                var pages = conn.GetPage<BlogTag>(null, new List<ISort> { Predicates.Sort<BlogTag>(s => s.Id, ascending) }, pageIndex - 1, pageSize);             
                return pages;
            }
        }
    }
}

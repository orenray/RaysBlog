using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Model
{
    public class EntityInfo<T> where T:class,new()
    {
        public IEnumerable<T> Entities { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalRecords { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// 当前页数
        /// </summary>
        public int CurrentPage { get; set; }
        public EntityInfo()
        {

        }
    }
}

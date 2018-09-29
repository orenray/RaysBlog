using System;
using System.Collections.Generic;

namespace RaysBlog.Model
{
    public class BlogArticle
    {
        public int ArticleId { get; set; }
        public string Body { get; set; }
        public BlogCategory Category { get; set; }
        public BlogTag Tag { get; set; }
        public DateTime PostDate { get; set; }
        public string Remark { get; set; }
        public string ArticleName { get; set; }
        public bool IsPublished { get; set; }
    }
}

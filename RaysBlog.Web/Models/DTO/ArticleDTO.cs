using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Web.Models.DTO
{
    public class ArticleDTO
    {
        public int ArticleId { get; set; }
        public string Body { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TagId { get; set; }
        public string  TagName { get; set; }
        public string PostDate { get; set; }
        public string Remark { get; set; }
        public string ArticleName { get; set; }
        public bool IsPublished { get; set; }
    }
}

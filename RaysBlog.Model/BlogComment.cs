using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Model
{
    public class BlogComment
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Content { get; set; }
        public int ParentId { get; set; }
    }
}

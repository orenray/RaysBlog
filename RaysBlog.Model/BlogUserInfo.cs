using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Model
{
    public class BlogUserInfo
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsEnabled { get; set; }
        public BlogPermission Permission { get; set; }
    }
}

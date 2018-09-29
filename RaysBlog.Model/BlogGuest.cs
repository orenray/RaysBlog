using System;
using System.Collections.Generic;
using System.Text;

namespace RaysBlog.Model
{
    public class BlogGuest
    {
        public int Id { get; set; }
        public string IP { get; set; }
        public string GuestName { get; set; }
        public string QQ { get; set; }
        public string Email { get; set; }
        public string No { get; set; }
        public string Remark { get; set; }
        public DateTime CreateDate { get; set; }
        public int CommentId { get; set; }
    }
}

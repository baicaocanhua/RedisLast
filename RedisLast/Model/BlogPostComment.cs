using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedisLast.Model
{
    public class BlogPostComment
    {
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
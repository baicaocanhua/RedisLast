using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedisLast.Model
{
    public class User
    {
        public User()
        {
            this.BlogIds = new List<long>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public List<long> BlogIds { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace utility
{
    public class UsageAttribute:Attribute
    {
        public UsageAttribute(string description)
        {
            this.Description = description;
        }

        public string Description { get; set; }
    }
}
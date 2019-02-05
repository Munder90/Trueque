using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TAT001.Models
{
    public class SelectTreeItem
    {
        public string text { get; set; }
        public string value { get; set; }
        public bool expanded { get; set; }
        public List<SelectTreeItem> items { get; set; }
    }
}
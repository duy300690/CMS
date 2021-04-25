using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMSWeb.Models
{
    [Serializable]
    public class EnumModel
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public int Key { get; set; }
        public byte Order { get; set; }
    }
}
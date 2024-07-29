using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorLogger.Models
{
    public class Embed
    {
        public string title { get; set; }  
        public int color { get; set; }
        public string description { get; set; }
        public string timestamp { get; set; }
        public string url { get; set; }
        public Dictionary<string, string> author { get; set; }
        public Dictionary<string, string> image { get; set; }
        public Dictionary<string, string> thumbnail { get; set; }
        public Dictionary<string, string> footer { get; set; }
        public List<Field> fields { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MADD
{
   public class GetFilter
    {
        public string name { get; set; }
        public string description { get; set; }
        public string folderName { get; set; }
        public string documentName { get; set; }
        public int folderType { get; set; }
        public int documentType { get; set; }
        public string database { get; set; }
        public bool abiEnabled { get; set; }
        public bool markEnabled { get; set; }
        public bool exportEnabled { get; set; }
    }
}

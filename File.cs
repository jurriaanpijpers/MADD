using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MADD
{
   public class File
    {
        public string id { get; set; }
        public int runNumber { get; set; }
        public int folderType { get; set; }
        public int documentType { get; set; }
        public string folderName { get; set; }
        public string documentName { get; set; }
        public string format { get; set; }
        public object filePath { get; set; }
        public object filename { get; set; }
        public object creationDate { get; set; }
        public object expirationDate { get; set; }
        public int size { get; set; }
        public string category { get; set; }
        public string folderNamePrefix { get; set; }
        public string folderNameSuffix { get; set; }
        public string database { get; set; }
        public string inputDescriptor { get; set; }
        public string prd { get; set; }
        public List<object> indexes { get; set; }
        public bool marked1 { get; set; }
        public bool opened { get; set; }
        public bool downloaded { get; set; }
        public bool printed { get; set; }
        public bool marked5 { get; set; }
        public bool marked6 { get; set; }
        public bool showMarked1 { get; set; }
        public bool showOpened { get; set; }
        public bool showDownloaded { get; set; }
        public bool showPrinted { get; set; }
        public bool showMarked5 { get; set; }
        public bool showMarked6 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Lecturer.Data.DataProcessor
{
    class LoadDataFromServer
    {
        private static XDocument doc { get; set; }

        public static string ReadFileFromServer()
        {
            string format = null;
            string textFromFile = null;
            WebRequest webRequest = HttpWebRequest.Create("https://www.dropbox.com/s/7y8b9xfyoje03vl/adr.txt?raw=1");
            using (WebResponse webResponse = webRequest.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            using (StreamReader sr = new StreamReader(stream))
            {
                textFromFile = sr.ReadToEnd();
                format = Path.GetExtension(webRequest.RequestUri.ToString());
            }


            return textFromFile;
        }
        
    }
}

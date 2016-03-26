using Lecturer.Data.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;


namespace Lecturer.Data.Processor
{
    public class StorageProcessor
    {
        private static XDocument doc { get; set; }
        public static string ServerPath = "D:\\FIleServer\\ИФМИТ";

        //public StorageProcessor

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

        public static string CreateDirectory(string root, string subfolder)
        {
            var path = Path.Combine(root, subfolder);
            try
            {
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                    //Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

                    // Delete the directory.
                    //di.Delete();
                    //Console.WriteLine("The directory was deleted successfully.");
                }
                return path;
            }
            catch
            {
                return "";
            }

        }

        public static string LoadFile(string loadPath, string savePath)
        {
            try
            {
                string filepath;



                return "";
            }
            catch
            {
                return "";
            }
        }


        public static void ProcessSchedule()
        {
            string[] dirs = Directory.GetFiles(ServerPath);

            var path = dirs.Where(file => file.Contains("xlst") || file.Contains("xls")).SingleOrDefault();
            ExcelFileProcessor fp = new ExcelFileProcessor(path, Cource.MyCource.GroupName);
            Cource.MyCource.Subjects = fp.FillSource();
        }

        public static string GetFilePath(string extension)
        {
            try
            {
                var subj = Cource.MyCource.SelectedSubject;
                string uri = Path.Combine(Cource.MyCource.RootFolderPath,
                        //Cource.MyCource.Semester,
                        subj.Name,
                        subj.SelectedTopic.Name);

                string[] dirs = Directory.GetFiles(uri);
                var path = dirs.Where(file => file.Contains(extension)).SingleOrDefault();
                return path;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Список тем (папок с темами)
        /// </summary>
        /// <param name="path">путь к директории дисциплины</param>
        /// <returns>спиок дисциплин</returns>
        public static List<Topic> GetTopicNames(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            List<Topic> topics = new List<Topic>();
            try
            {
                foreach (var directory in dirs)
                {
                    var str = directory.Split('\\').LastOrDefault();
                    topics.Add(new Topic
                    {
                        Name = str,
                        IsStudied = false
                    });
                }

                return topics;
            }
            catch
            {
                return null;
            }
        }
    }
}

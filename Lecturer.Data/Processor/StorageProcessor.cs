using Lecturer.Data.Entities;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;


namespace Lecturer.Data.Processor
{
    public class StorageProcessor
    {
        private static XDocument doc { get; set; }
        private static string username = "2lecturer";
        private static string password = "student12345";
        private static string uri = "ftp://lecturer.at.ua/";


        public static string ReplaceCharacters(string uri, bool flag)
        {
            if (flag == false)
                return uri.Replace('і', '_').Replace('І', '_');
            else return uri.Replace('_', 'і');
        }
        

        /// <summary>
        /// Подготовка потока для чтения содержимого директории
        /// </summary>
        /// <param name="path">путь на сервере</param>
        /// <returns>поток</returns>
        private static StreamReader ListDirectoryOnServer(string path)
        {
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(path);
                ftpRequest.Credentials = new NetworkCredential(username, password);
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                StreamReader streamReader = new StreamReader(response.GetResponseStream());

                return streamReader;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Подготовка потока для скачивания файла
        /// </summary>
        /// <param name="path">путь на сервере</param>
        /// <returns>поток</returns>
        private static FtpWebResponse LoadFileFromPath(string path)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                request.Credentials = new NetworkCredential(username, password);
                request.UseBinary = true; // Use binary to ensure correct dlv!
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return response;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Скачивание файла по FTP
        /// </summary>
        /// <param name="subpath">расположение на сервере (подпапки)</param>
        /// <param name="localPath">путь, по которому файл будет сохранен</param>
        /// <param name="extensions">список возможных расширений файла</param>
        /// <returns>путь к файлу</returns>
        public static string TryGetFileByFTP(string subpath, string localPath, string[] extensions)
        {
           
            try {

                string ftpAddr = uri + subpath;
                string line = null;

                //поиск файла с указанным расширением 
                var fileSeek = ListDirectoryOnServer(ftpAddr);
                string filename = null;
                line = fileSeek.ReadLine();
                while (!string.IsNullOrEmpty(line))
                {
                    for (int i = 0; i < extensions.Count(); i++)
                        if (line.Contains(extensions[i]))
                        {
                            filename = line;
                            break;
                        }
                    if (filename != null)
                    {
                        ftpAddr = ftpAddr + filename;
                        break;
                    }
                    line = fileSeek.ReadLine();
                }
                fileSeek.Close();

                //скачивание файла
                var response = LoadFileFromPath(ftpAddr);
                if (response != null)
                {
                    Stream responseStream = response.GetResponseStream();
                    string loadedFilePath = Path.Combine(localPath, filename);
                    FileStream writer = new FileStream(loadedFilePath, FileMode.Create);

                    long length = response.ContentLength;
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[2048];

                    readCount = responseStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        writer.Write(buffer, 0, readCount);
                        readCount = responseStream.Read(buffer, 0, bufferSize);
                    }
                    responseStream.Close();
                    response.Close();
                    writer.Close();
                    return loadedFilePath;
                }

                return null;
            }
            catch(System.Exception ex)
            {
                return null;
            }
        }


        public static bool ProcessZipFile(string fullFilePath, string extractPath)
        {
            try
            {
                if (fullFilePath.Contains("zip"))
                {
                    var enco = Encoding.GetEncoding("cp866");
                    ZipFile.ExtractToDirectory(fullFilePath, extractPath, enco);
                    File.Delete(fullFilePath);
                    return true;

                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Создание папки
        /// </summary>
        /// <param name="root"></param>
        /// <param name="subfolder"></param>
        /// <returns></returns>
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


        public static void ProcessSchedule(Institute selectedIns)
        {
            string[] ext = { "xls", "xlst" };
            string subpath = selectedIns.FolderName + @"/";
            string pathToFile = TryGetFileByFTP(subpath, Cource.MyCource.RootFolderPath, ext);
            
            ExcelFileProcessor fp = new ExcelFileProcessor(pathToFile, Cource.MyCource.GroupName);
            Cource.MyCource.Subjects = fp.FillSource();
        }


        /// <summary>
        /// Поиск файла в папке темы (лекция, тест, т.п.)
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="extension">расширение</param>
        /// <returns>путь к файлу</returns>
        public static string GetFilePath(string path, string extension)
        {
            try
            {
                string[] dirs = Directory.GetFiles(path);
                var filepath = dirs.Where(file => file.Contains(extension)).SingleOrDefault();
                return filepath;
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
        public static List<Topic> GetFolderNames(string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                List<Topic> topics = new List<Topic>();
                foreach (var directory in dirs)
                {
                    var str = directory.Split('\\').LastOrDefault();
                    topics.Add(new Topic
                    {
                        Name = StorageProcessor.ReplaceCharacters(str, true),
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

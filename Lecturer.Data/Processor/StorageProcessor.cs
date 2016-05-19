using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


namespace Lecturer.Data.Processor
{
    public class StorageProcessor
    {
        /// <summary>
        /// Замена символов
        /// </summary>
        /// <param name="uri">адрес папки</param>
        /// <param name="flag">флаг вида замены</param>
        /// <returns>измененная строка</returns>
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
        /// <param name="username">имя пользователя</param>
        /// <param name="password">пароль</param>
        /// <returns>поток</returns>
        private static StreamReader ListDirectoriesOnServer(string path, string username, string password)
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
        /// <param name="username">имя пользователя</param>
        /// <param name="password">пароль</param>
        /// <returns>поток</returns>
        private static FtpWebResponse LoadFileFromPath(string path, string username, string password)
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
        /// асинхронная загрузка данных с сервера
        /// </summary>
        /// <returns>выполняемая операция</returns>
        public static Task GetSemesterFilesAsync()
        {
            return Task.Run(() =>
            {
                var folder = new DirectoryInfo(Path.Combine(Course.MyCourse.RootFolderPath, Course.MyCourse.Semester));
                if (!folder.Exists)
                {
                    //загрузка данных с сервера
                    string[] ext = { "zip" };
                    string filename = TryGetFileByFTP(Course.MyCourse.GetServerSubpath, Course.MyCourse.RootFolderPath, ext);
                    bool flag = ProcessZipFile(filename, Course.MyCourse.RootFolderPath);

                }
                    //загрузка расписания с сервера
                    ProcessSchedule(Course.MyCourse.InstituteCode);
            });
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
            try
            {
                var xmlProc = new XMLProcessor("University.xml");
                string username = xmlProc.XFile.Root.Attribute("username").Value;
                string password = xmlProc.XFile.Root.Attribute("password").Value;
                string uri = xmlProc.XFile.Root.Attribute("server").Value;

                string ftpAddr = uri + subpath;
                string line = null;

                //поиск файла с указанным расширением 
                var fileSeek = ListDirectoriesOnServer(ftpAddr, username, password);
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
                var response = LoadFileFromPath(ftpAddr, username, password);
                if (response != null)
                {
                    Stream responseStream = response.GetResponseStream();
                    FileStream writer = new FileStream(Path.Combine(localPath, filename), FileMode.Create);

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
                    return filename;
                }

                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// обработка zip-архива
        /// </summary>
        /// <param name="filename">путь к файлу, включая название и расширение</param>
        /// <param name="extractPath">путь, по которому архив будет извлечен</param>
        /// <returns></returns>
        public static bool ProcessZipFile(string filename, string extractPath)
        {
            try
            {
                if (filename.Contains("zip"))
                {
                    var filepath = Path.Combine(Course.MyCourse.RootFolderPath, filename);
                    var enco = Encoding.GetEncoding("cp866");
                    ZipFile.ExtractToDirectory(filepath, extractPath, enco);
                    File.Delete(filepath);
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
        /// <param name="root">корневая папка для создаваемой</param>
        /// <param name="subfolder">название создаваемой папки</param>
        /// <returns></returns>
        public static string CreateDirectory(string root, string subfolder)
        {
            var path = Path.Combine(root, subfolder);
            try
            {
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }
                return path;
            }
            catch
            {
                return "";
            }

        }
        
        /// <summary>
        /// Обработка файла с расписанием (списком дисциплин)
        /// </summary>
        /// <param name="instituteCode">код института/факультета</param>
        public static void ProcessSchedule(string instituteCode)
        {
            //скачивание
            string[] ext = { "xls", "xlst" };
            string subpath = instituteCode + @"/";
            string filename = TryGetFileByFTP(subpath, Course.MyCourse.RootFolderPath, ext);

            //разбор и сохранение данных локально
            string schedulePath = Path.Combine(Course.MyCourse.RootFolderPath, filename);
            ExcelFileProcessor fp = new ExcelFileProcessor(schedulePath, Course.MyCourse.GroupName);
            Course.MyCourse.Subjects = fp.FillSource();

            //удаление
            FileInfo fi = new FileInfo(schedulePath);
            fi.Delete(); 
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
        /// <returns>список тем</returns>
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

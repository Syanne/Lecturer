using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Lecturer.Data.Entities;
using System.Windows.Navigation;
using Lecturer.Data.Processor;
using System.IO;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for PersonalData.xaml
    /// </summary>
    public partial class UserDataPage : Page
    {
        List<UserData> uData;
        int SelectedIndex { get; set; }
        XMLProcessor xProc { get; set; }

        public UserDataPage()
        {
            InitializeComponent();
            uData = new List<UserData>();
            xProc = new XMLProcessor("settings.xml");

            xProc.PrepareUserData(ref uData);
            lvPersonalData.ItemsSource = uData;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CoursePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnOnListView_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = (int)(sender as Button).Tag;

            var instance = uData[SelectedIndex];
            InputDialog inputDialog = new InputDialog(instance.Title, instance.Value);


            if (instance.Key != "location" && instance.Key != "remove")
            {
                if (inputDialog.ShowDialog() == true)
                {

                    if (inputDialog.Answer != "" && inputDialog.Answer != instance.Value)
                    {
                        //строковые данные
                        if (instance.Key != "courceNumber")
                        {
                            WriteDataIntoFile(inputDialog.Answer);
                        }
                        //числовые данные - семестр
                        else
                        {
                            try
                            {
                                int courceValue = Convert.ToInt32(inputDialog.Answer);
                                if (courceValue <= 5)
                                {
                                    LoadFilesFromServer(courceValue);
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            else if(instance.Key == "remove")
            {
                if (MessageBox.Show("Ви дійсно бажайте вийти з акаунта та видалити всі дані? Файли з навчальними матеріалами не буде видалено", "Увага!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    File.Delete("settings.xml");
                    Application.Current.Shutdown();
                }
            }
            //путь к директории с файлами
            else 
            {
                System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();

                folderDlg.ShowNewFolderButton = true;

                // Show the FolderBrowserDialog
                System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {       
                    //копирование директории и содержимого,
                    //затем - удаление директории и старого расположения             
                    CopyDirectory(instance.Value, System.IO.Path.Combine(folderDlg.SelectedPath, "Лектор"));
                    DirectoryInfo dir = new DirectoryInfo(instance.Value);
                    dir.Delete(true);

                    //меняем путь к директории
                    Course.MyCourse.RootFolderPath = System.IO.Path.Combine(folderDlg.SelectedPath, "Лектор");
                    WriteDataIntoFile(System.IO.Path.Combine(folderDlg.SelectedPath, "Лектор"));

                    instance.Value = System.IO.Path.Combine(folderDlg.SelectedPath, "Лектор");
                }                
            }
        }

        /// <summary>
        /// Подготовка к загрузке данных 
        /// </summary>
        /// <param name="courceValue">номер курса</param>
        private async void LoadFilesFromServer(int courceValue)
        {
            loadingGrid.Visibility = Visibility.Visible;
            Course.MyCourse.CourseNumber = courceValue.ToString();

            //проверяем текущий семестр (летние месяцы не относятся ни к одному из них)
            int semester = 0;
            if (DateTime.Now.Month > 1 && DateTime.Now.Month <= 5)
                semester = courceValue * 2;
            else if (DateTime.Now.Month >= 9)
                semester = courceValue * 2 - 1;

            //сохраняем данные в файл с настройками и загружаем данные с сервера
            if (semester != 0)
            {
                Course.MyCourse.Semester = semester.ToString();

                WriteDataIntoFile(courceValue.ToString());

                //асинхронная загрузка данных с сервера
                await StorageProcessor.GetSemesterFilesAsync();

                xProc.WriteSemester();
                Course.MyCourse.Subjects = xProc.GetSubjectList();
            }
                loadingGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Изменение файла с данными
        /// </summary>
        /// <param name="changedValue">измененное значение</param>
        private void WriteDataIntoFile(string changedValue)
        {
            var root = xProc.XFile.Root;

            root.Attribute(uData[SelectedIndex].Key).Value = changedValue;
            xProc.SaveDocument();
            uData[SelectedIndex].Value = changedValue;

            lvPersonalData.ItemsSource = null;
            lvPersonalData.ItemsSource = uData;
        }

        /// <summary>
        /// Копирование папки 
        /// </summary>
        /// <param name="startFolder">папка, которая будет скопирована</param>
        /// <param name="targetFolder">путь, по которому будет скопирована папка</param>
        private void CopyDirectory(string startFolder, string targetFolder)
        {
            //исхдная директория
            DirectoryInfo dir_inf = new DirectoryInfo(startFolder);

            //перебор вложенных папок
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())
            {
                if (Directory.Exists(targetFolder + "\\" + dir.Name) != true)
                {
                    Directory.CreateDirectory(targetFolder + "\\" + dir.Name);
                }

                //перебор всех вложенных файлов и папок
                CopyDirectory(dir.FullName, targetFolder + "\\" + dir.Name);
            }

            //файлы в папке
            foreach (string file in Directory.GetFiles(startFolder))
            {
                //имя файла без пути
                string fileWithoutPath = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));

                //Копируем файл с перезаписью из источника в приёмник.
                File.Copy(file, targetFolder + "\\" + fileWithoutPath, true);
            }
        }
    }
}

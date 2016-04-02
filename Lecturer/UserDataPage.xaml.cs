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
        int SelectedIndex;
        XMLProcessor xProc;

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
            nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnOnListView_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = (int)(sender as Button).Tag;

            var instance = uData[SelectedIndex];
            InputDialog inputDialog = new InputDialog(instance.Title, instance.Value);


            if (instance.Key != "location")
            {
                if (inputDialog.ShowDialog() == true)
                {
                    if (inputDialog.Answer != "" && inputDialog.Answer != instance.Value)
                    {
                        //строковые данные
                        if (instance.Key != "semester")
                        {
                            ProcessAction(inputDialog.Answer);
                        }
                        //числовые данные - семестр
                        else
                        {
                            try
                            {
                                if (Convert.ToInt32(inputDialog.Answer) < 12)
                                {
                                    ProcessAction(inputDialog.Answer);

                                    XMLProcessor processor = new XMLProcessor("settings.xml");
                                    Cource.MyCource.Subjects = processor.GetSubjectList();
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
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
                    CopyDirectory(instance.Value, System.IO.Path.Combine(folderDlg.SelectedPath, "Лектор"));
                    DirectoryInfo dir = new DirectoryInfo(instance.Value);
                    dir.Delete(true);
                    Cource.MyCource.RootFolderPath = System.IO.Path.Combine(folderDlg.SelectedPath, "Лектор", Cource.MyCource.Semester);
                    ProcessAction(System.IO.Path.Combine(folderDlg.SelectedPath, "Лектор"));

                    instance.Value = System.IO.Path.Combine(folderDlg.SelectedPath, "Лектор");
                }                
            }
        }

        /// <summary>
        /// Изменение файла с данными
        /// </summary>
        /// <param name="answer">измененное значение</param>
        private void ProcessAction(string answer)
        {
            var root = xProc.PersonalData.Root;

            root.Attribute(uData[SelectedIndex].Key).Value = answer;
            xProc.SaveDocument();
            uData[SelectedIndex].Value = answer;

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
            //Берём нашу исходную папку
            DirectoryInfo dir_inf = new DirectoryInfo(startFolder);
            //Перебираем все внутренние папки
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())
            {
                //Проверяем - если директории не существует, то создаём;
                if (Directory.Exists(targetFolder + "\\" + dir.Name) != true)
                {
                    Directory.CreateDirectory(targetFolder + "\\" + dir.Name);
                }

                //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
                CopyDirectory(dir.FullName, targetFolder + "\\" + dir.Name);
            }

            //Перебираем файлы в источнике.
            foreach (string file in Directory.GetFiles(startFolder))
            {
                //Оотделяем имя файла с расширением - без пути (но с слешем "\").
                string fileWithoutPath = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));

                //Копируем файл с перезаписью из источника в приёмник.
                File.Copy(file, targetFolder + "\\" + fileWithoutPath, true);
            }
        }
    }
}

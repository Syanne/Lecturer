using Lecturer.Data.Processor;
using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (Password.Password.Count() >= 8 && 
                UserName.Text.Count() >= 5 &&
                Folder.Text != "")
            {
                ProcessUserFile();
            }

        }

        private void ProcessUserFile()
        {            
            XMLProcessor processor = new XMLProcessor("settings.xml");
            if (processor.PersonalData == null)
            {
                //mock
                Cource.MyCource.Semester = "8";
                Cource.MyCource.GroupName = "4 ПІ";
                Cource.MyCource.Speciality = "ПІ";
                //-----mock

                //создание файла с настройками
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                dictionary.Add("username", UserName.Text);
                dictionary.Add("password", Password.Password);
                dictionary.Add("semester", Cource.MyCource.Semester);
                dictionary.Add("location", Cource.MyCource.RootFolderPath);
                dictionary.Add("speciality", Cource.MyCource.Speciality);

                processor.CreateSettingsFile(dictionary);
                StorageProcessor.ProcessSchedule();
                processor.FillSemester();
            }

            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
        }
               
        private void Folder_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = true;

            // Show the FolderBrowserDialog
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Folder.Text = folderDlg.SelectedPath;
                Cource.MyCource.RootFolderPath = StorageProcessor.CreateDirectory(folderDlg.SelectedPath, "Лектор");
            }
            //else Folder.Text = "";
        }
        
    }
}

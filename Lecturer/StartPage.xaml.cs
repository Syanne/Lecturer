using Lecturer.Data.DataProcessor;
using Lecturer.Data.Entities;
using Lecturer.Data.Processor;
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
            if (Password.Password.Count() >= 8 && UserName.Text.Count() >= 5)
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
                //-----mock

                //создание файла с настройками
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                dictionary.Add("username", UserName.Text);
                dictionary.Add("password", Password.Password);
                dictionary.Add("semester", Cource.MyCource.Semester);

                processor.CreateSettingsFile(dictionary);
                ProcessSchedule();
                processor.FillSemester();
            }

            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void ProcessSchedule()
        {
            ExcelFileProcessor fp = new ExcelFileProcessor(@"repo/graph.xls", Cource.MyCource.GroupName);
            Cource.MyCource.Subjects = fp.FillSource();
        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckEnabled();
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckEnabled();
        }

        private void CheckEnabled()
        {
            //if (Password.Password.Count() >= 8 && UserName.Text.Count() >= 5)
            //{
            //    Done.IsEnabled = true;
            //}
        }
    }
}

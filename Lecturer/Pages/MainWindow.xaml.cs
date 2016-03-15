using Lecturer.Data.DataProcessor;
using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Lecturer.Pages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Cource MyCource { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
            if (TryLoadUserData() == true)
                mainFrame.Source = new Uri("CourcePage.xaml", UriKind.Relative);       
            else mainFrame.Source = new Uri("StartPage.xaml", UriKind.Relative);

        }

        /// <summary>
        /// Загрузка персональных данных пользователя
        /// </summary>
        /// <returns>удалось ли загрузить файл</returns>
        private bool TryLoadUserData()
        {
            XMLProcessor processor = new XMLProcessor("settings.xml");

            if (File.Exists("settings.xml"))
            {
                string username = processor.ReadValue(false, "userdata", "username");
                string password = processor.ReadValue(false, "userdata", "password");
                if (username == "" || password == "")
                    return false;

                Cource.MyCource.Subjects = new List<Subject>();
                Cource.MyCource.CourceNumber = processor.ReadValue(false, "userdata", "cource");
                Cource.MyCource.Semester = processor.ReadValue(false, "userdata", "semester");

                return true;
            }
            else return false;
        }
    }
}

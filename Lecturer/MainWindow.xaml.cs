using Lecturer.Data.Processor;
using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Lecturer
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
        /// Загрузка личных данных пользователя
        /// </summary>
        /// <returns>удалось ли загрузить файл</returns>
        private bool TryLoadUserData()
        {
            XMLProcessor processor = new XMLProcessor("settings.xml");

            if (File.Exists("settings.xml"))
            {
                Cource.MyCource.Subjects = new List<Subject>();
                Cource.MyCource.Semester = processor.PersonalData.Root.Attribute("semester").Value;
                Cource.MyCource.Subjects = processor.GetSubjectList();
                Cource.MyCource.RootFolderPath = Path.Combine(processor.PersonalData.Root.Attribute("location").Value,
                                                                processor.PersonalData.Root.Attribute("semester").Value);

                return true;
            }
            else return false;
        }

        private void mainFrame_ContentRendered(object sender, EventArgs e)
        {
            mainFrame.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
        }
    }
}

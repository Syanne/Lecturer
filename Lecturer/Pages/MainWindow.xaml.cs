using Lecturer.Data.DataProcessor;
using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Shapes;

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

        private bool TryLoadUserData()
        {
            XMLFileProcessor processor = new XMLFileProcessor("settings.xml");

            if (File.Exists("settings.xml"))
            {
                string username = processor.ReadValue("userdata", "username");
                string password = processor.ReadValue("userdata", "password");
                if (username == "" || password == "")
                    return false;

                Cource.MyCource.Subjects = new List<Subject>();
                Cource.MyCource.CourceNumber = processor.ReadValue("userdata", "cource");
                Cource.MyCource.Semester = processor.ReadValue("userdata", "semester");

                return true;
            }
            else return false;
        }



        private void ConnectToFolder()
        {
            string path = System.IO.Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDoc‌​uments),
                    "Lecturer",
                    "settings.dat");
            if (Directory.Exists(path))
            {

            }
            else
            {
                //string patth = System.IO.Path.GetFullPath(Environment.SpecialFolder.MyDocuments.ToString());
                //Directory.CreateDirectory(System.IO.Path.Combine(Environment.SpecialFolder.MyDocuments.ToString(), "Lecturer"));
            }
        }
    }
}

using Lecturer.Data.Entities;
using Lecturer.Data.Processor;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for LectionPage.xaml
    /// </summary>
    public partial class LectionPage : Page
    {
        public LectionPage()
        {
            InitializeComponent();

            pdfControl.Filepath = Course.MyCourse.SelectedSubject.SelectedTopic.LectionUri;         
   
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            
            string uri = Path.Combine(Course.MyCourse.RootFolderPath, 
                    Course.MyCourse.Semester,
                    StorageProcessor.ReplaceCharacters(Course.MyCourse.SelectedSubject.Name, false),
                    StorageProcessor.ReplaceCharacters(Course.MyCourse.SelectedSubject.SelectedTopic.Name, false));

            var path = StorageProcessor.GetFilePath(uri, "xml");
            //файл с тестом не найден
            if (Course.MyCourse.SelectedSubject.SelectedTopic.IsStudied == true || path == null)
            {
                XMLProcessor xProc = new XMLProcessor("settings.xml");
                xProc.SetTopicStudied();

                Course.MyCourse.SelectedSubject.SelectedTopic.IsStudied = true;
                Course.MyCourse.SelectedSubject.SelectedTopic = null;

                nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));
            }

            //переход на страницу тестирования
            else
            {
                Course.MyCourse.SelectedSubject.SelectedTopic.TestUri = path;
                nav.Navigate(new Uri("TestPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}

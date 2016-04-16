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

            pdfControl.Filepath = Cource.MyCource.SelectedSubject.SelectedTopic.LectionUri;         
   
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            
            string uri = Path.Combine(Cource.MyCource.RootFolderPath, 
                    Cource.MyCource.Semester,
                    StorageProcessor.ReplaceCharacters(Cource.MyCource.SelectedSubject.Name, false),
                    StorageProcessor.ReplaceCharacters(Cource.MyCource.SelectedSubject.SelectedTopic.Name, false));

            var path = StorageProcessor.GetFilePath(uri, "xml");
            //файл с тестом не найден
            if (Cource.MyCource.SelectedSubject.SelectedTopic.IsStudied == true || path == null)
            {
                XMLProcessor xProc = new XMLProcessor("settings.xml");
                xProc.SetTopicStudied();

                Cource.MyCource.SelectedSubject.SelectedTopic.IsStudied = true;
                Cource.MyCource.SelectedSubject.SelectedTopic = null;

                nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));
            }

            //переход на страницу тестирования
            else
            {
                Cource.MyCource.SelectedSubject.SelectedTopic.TestUri = path;
                nav.Navigate(new Uri("TestPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}

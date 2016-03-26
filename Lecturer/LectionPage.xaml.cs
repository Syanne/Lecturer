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

            pdfControl.FilePath = Cource.MyCource.SelectedSubject.SelectedTopic.LectionUri;         
   
        }


        private void Next_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            var path = StorageProcessor.GetFilePath("xml");
            if (Cource.MyCource.SelectedSubject.SelectedTopic.IsStudied == true || path == null)
            {

                XMLProcessor xProc = new XMLProcessor("settings.xml");
                xProc.SetTopicStudied();
                Cource.MyCource.SelectedSubject.SelectedTopic.IsStudied = true;

                Cource.MyCource.SelectedSubject.SelectedTopic = null;
                nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                Cource.MyCource.SelectedSubject.SelectedTopic.TestUri = path;
                nav.Navigate(new Uri("TestPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}

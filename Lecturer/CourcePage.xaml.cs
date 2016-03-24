using Lecturer.Data.Processor;
using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for CourcePage.xaml
    /// </summary>
    public partial class CourcePage : Page
    {
        public CourcePage()
        {
            InitializeComponent();

            PrepareData();

            ShowsNavigationUI = false;
        }

        /// <summary>
        /// готовим данные для вывода на экран
        /// </summary>
        private void PrepareData()
        {
            if (Cource.MyCource.Subjects == null)
            {
                XMLProcessor xProc = new XMLProcessor("settings.xml");

                Cource.MyCource.Subjects = xProc.GetSubjectList();
            }

            if (Cource.MyCource.Subjects == null)
                Cource.MyCource.Subjects = new List<Subject>()
                {
                    new Subject()
                    {   Name = "Рассписание не найдено",
                        Hours = "--"
                    }
                };
            myList.ItemsSource = Cource.MyCource.Subjects;                        
        }        


        private void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            myList.SelectedItem = null;
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));

        }
    }
}

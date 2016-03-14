using Lecturer.Data.Entities;
using Lecturer.Data.Processor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Lecturer.Pages
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
        }

        /// <summary>
        /// готовим данные для вывода на экран
        /// </summary>
        private void PrepareData()
        {
            DataBaseFileParser fp = new DataBaseFileParser("ДІД.xlsx", Cource.MyCource.Semester);
            fp.FillSource(Cource.MyCource.Subjects);

            if (Cource.MyCource.Subjects == null)
                Cource.MyCource.Subjects = new List<Subject>() { new Subject() { Name = "Рассписание не найдено", Hours = "--" } };
            myList.ItemsSource = Cource.MyCource.Subjects;            
        }


        private void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("Pages/SubjectPage.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}

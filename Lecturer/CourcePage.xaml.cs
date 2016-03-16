using Lecturer.Data.Entities;
using Lecturer.Data.Processor;
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
            Cource.MyCource.Subjects = new List<Subject>();
            ExcelFileProcessor fp = new ExcelFileProcessor("ДІД.xlsx", Cource.MyCource.Semester);
            Cource.MyCource.Subjects = fp.FillSource();

            if (Cource.MyCource.Subjects == null)
                Cource.MyCource.Subjects = new List<Subject>() { new Subject() { Name = "Рассписание не найдено", Hours = "--" } };
            myList.ItemsSource = Cource.MyCource.Subjects;            
        }


        private void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            myList.SelectedItem = null;
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("LectionPage.xaml", UriKind.RelativeOrAbsolute));

        }

    }
}

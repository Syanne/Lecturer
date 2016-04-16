using Lecturer.Data.Processor;
using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Linq;

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

        private void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (myList.SelectedItem != null)
            {
                var selectedItem = (sender as ListView).SelectedItem as Subject;

                //ищем список тем в файле 
                try
                {
                    XMLProcessor xProc = new XMLProcessor("settings.xml");
                    selectedItem.Topics = xProc.GetTopicList(selectedItem.Name);

                    if(selectedItem.Topics == null)
                        throw new Exception();

                }

                //иначе - в папке программы на диске
                catch (Exception ex)
                {
                    var folderPath = System.IO.Path.Combine(Cource.MyCource.RootFolderPath, Cource.MyCource.Semester, StorageProcessor.ReplaceCharacters(selectedItem.Name, false));
                    selectedItem.Topics = StorageProcessor.GetFolderNames(folderPath);

                    if (selectedItem.Topics != null)
                    {
                        XMLProcessor xProc = new XMLProcessor("settings.xml");
                        xProc.FillTopicList(selectedItem);
                    }
                }

                //переход на страницу дисциплины
                finally
                {
                    if (selectedItem.Topics != null)
                    {
                        Cource.MyCource.SelectedSubject = (sender as ListView).SelectedItem as Subject;

                        NavigationService nav = NavigationService.GetNavigationService(this);
                        nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        myList.SelectedItem = null;
                    }
                }
            }
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("UserDataPage.xaml", UriKind.RelativeOrAbsolute));
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


                if (Cource.MyCource.Subjects == null)
                    Cource.MyCource.Subjects = new List<Subject>()
                    {
                        new Subject()
                        {   Name = "Рассписание не найдено",
                            Hours = "--"
                        }
                    };
            }

            myList.ItemsSource = Cource.MyCource.Subjects;
        }
    }
}

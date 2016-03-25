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
            var selectedItem = (sender as ListView).SelectedItem as Subject;
            try
            {
                XMLProcessor xProc = new XMLProcessor("settings.xml");
                selectedItem.Topics = new List<Topic>();

                //ищем список тем
                var items = xProc.PersonalData.Root.Elements("semester")
                    .FirstOrDefault(elem => elem.Attribute("number").Value == Cource.MyCource.Semester)
                    .Elements("subject")
                    .SingleOrDefault(elem => elem.Attribute("name").Value == selectedItem.Name)
                    .Elements("topic");

                if (items == null || items.Count() == 0)
                    throw new Exception();

                foreach (var item in items)
                {
                    selectedItem.Topics.Add(new Topic
                    {
                        Name = item.Attribute("name").Value,
                        IsStudied = item.Attribute("isStudied").Value
                    });
                }

                Cource.MyCource.SelectedSubj = (sender as ListView).SelectedIndex;

               // myList.SelectedIndex = -1;
                NavigationService nav = NavigationService.GetNavigationService(this);
                nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));
                
            }
            catch(Exception ex)
            {
                var folderPath = System.IO.Path.Combine(Cource.MyCource.RootFolderPath, selectedItem.Name);
                selectedItem.Topics = StorageProcessor.GetTopicNames(folderPath);

                XMLProcessor xProc = new XMLProcessor("settings.xml");
                if (selectedItem.Topics != null)
                    xProc.FillTopic(selectedItem);

                Cource.MyCource.SelectedSubj = (sender as ListView).SelectedIndex;
                //myList.SelectedItem = null;
                NavigationService nav = NavigationService.GetNavigationService(this);
                nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}

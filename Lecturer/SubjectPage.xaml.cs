using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for SubjectPage.xaml
    /// </summary>
    public partial class SubjectPage : Page
    {
        public SubjectPage()
        {
            InitializeComponent();
            int index = Cource.MyCource.SelectedSubj;
            

            int counter = 0;
            foreach(var item in Cource.MyCource.Subjects[index].Topics)
            {
                if (counter < 1)
                {
                    counter = (item.IsStudied == true) ? 0 : counter + 1;
                    item.Opacity = 1.0;
                }
                else
                {
                    item.Opacity = 0.8;
                }

                if (item.IsStudied == true)
                    item.CircleColor = (Brush)this.Resources["MainColorBrush"];
                else if (item.Opacity == 1.0)
                    item.CircleColor = (Brush)this.Resources["RedBrush"];
                else item.CircleColor = new SolidColorBrush(Colors.Gray);
            }

            btnLink.Content = "< "+Cource.MyCource.Subjects[index].Name;
            myList.ItemsSource = Cource.MyCource.Subjects[index].Topics;
        }


        private void PopupButton_Click(object sender, RoutedEventArgs e)
        {
            switch((sender as Button).Tag.ToString())
            {
                case "0": break;
                case "1": break;
                default: ActionPopup.IsOpen = false;
                    break;
            }
        }

        private void myList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void subjectList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
        }
    }

}

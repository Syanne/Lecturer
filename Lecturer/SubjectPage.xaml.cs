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
    }
}

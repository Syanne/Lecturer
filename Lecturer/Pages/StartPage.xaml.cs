using Lecturer.Data.DataProcessor;
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
using System.Windows.Shapes;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            XMLFileProcessor processor = new XMLFileProcessor("settings.xml");

            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            dictionary.Add("username", UserName.Text);
            dictionary.Add("password", Password.Password);
            dictionary.Add("cource", "1");
            dictionary.Add("semester", "1");

            processor.CreateSettingsFile(dictionary);


            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("Pages/CourcePage.xaml", UriKind.RelativeOrAbsolute));

        }

        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckEnabled();
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            CheckEnabled();
        }

        private void CheckEnabled()
        {
            if (Password.Password.Count() >= 8 && UserName.Text.Count() >= 5)
            {
                Done.IsEnabled = true;
            }
        }
    }
}

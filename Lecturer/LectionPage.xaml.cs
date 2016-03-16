using Lector.DataProcessor.DataProcessor;
using Lector.DataProcessor.Processor;
using Lecturer.Data.Entities;
using Microsoft.Win32;
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
    /// Interaction logic for LectionPage.xaml
    /// </summary>
    public partial class LectionPage : Page
    {
        private bool canOpenFile;
        Topic topic;

        public LectionPage()
        {
            InitializeComponent();
            canOpenFile = CheckAcrobatInstallation();

            if (canOpenFile == false)
                ShowInstallationMessage();
            else
            {
                topic = new Topic
                {
                    Name = "Frikonomika",
                    ID = "0",
                    LectionUri = @"Frikonomika.pdf",
                    IsStudied = false,
                    Quiz = null
                };
                pdfControl.FilePath = topic.LectionUri;
            }
   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckAcrobatInstallation()
        {
            RegistryKey adobe = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("Adobe");
            if (null == adobe)
            {
                var policies = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("Policies");
                if (null == policies)
                {
                    return false;
                }
                adobe = policies.OpenSubKey("Adobe");
            }
            if (adobe != null)
            {
                RegistryKey acroRead = adobe.OpenSubKey("Acrobat Reader");
                if (acroRead != null)
                {
                    string[] acroReadVersions = acroRead.GetSubKeyNames();
                    return true;
                }
                else return false;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowInstallationMessage()
        {
            if (MessageBox.Show(
                "Adobe Acrobat Reader не установлен! Хотите установить программу, чтобы продолжить работу с приложением?",
                "Adobe Acrobat Reader",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start("https://get.adobe.com/ru/reader/");
            }
            else
            {

            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if(topic.Quiz == null)
            {
                NavigationService nav = NavigationService.GetNavigationService(this);
                nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
                
            }
        }
    }
}

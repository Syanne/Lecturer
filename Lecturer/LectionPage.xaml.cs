using Lecturer.Data.Entities;
using Microsoft.Win32;
using System;
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
                    LectionUri = @"repo/Frikonomika.pdf",
                    IsStudied = false,
                    HasTest = true
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
                    bool flag = false;
                    foreach(var item in acroReadVersions)
                    {
                        flag = (item.Contains("11.") == true) ? true : false;
                        if (flag == true)
                            break;
                    }
                    return flag;
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
                System.Diagnostics.Process.Start("AdobeReader11.exe");
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            if (topic.HasTest == false)
            {
                nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
            }
            else
            {
                nav.Navigate(new Uri("TestPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
    }
}

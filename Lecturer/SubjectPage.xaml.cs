﻿using Lecturer.Data.Entities;
using Lecturer.Data.Processor;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

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
            

            int counter = 0;
            foreach(var item in Cource.MyCource.SelectedSubject.Topics)
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

            btnLink.Content = "< "+Cource.MyCource.SelectedSubject.Name;
            myList.ItemsSource = Cource.MyCource.SelectedSubject.Topics;
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
            //
            if (((sender as ListView).SelectedItem as Topic).Opacity == 1.0)
            {
                //проверим, установлен ли reader
                var canOpenFile = CheckAcrobatInstallation();
                if (canOpenFile == false)
                    ShowInstallationMessage();
                else
                {
                    Cource.MyCource.SelectedSubject.SelectedTopic = (sender as ListView).SelectedItem as Topic;

                    string path = StorageProcessor.GetFilePath("pdf");

                    if (path != null)
                        Cource.MyCource.SelectedSubject.SelectedTopic.LectionUri = path;

                    NavigationService nav = NavigationService.GetNavigationService(this);
                    nav.Navigate(new Uri("LectionPage.xaml", UriKind.RelativeOrAbsolute));
                }
            }
        }
        

        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            Cource.MyCource.Subjects = null;
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));

        }
        /// <summary>
        /// Проверка наличия Adobe Reader
        /// </summary>
        /// <returns>true - установлен, false - нет</returns>
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
                    foreach (var item in acroReadVersions)
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
        /// Предложение установить ридер
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
    }

}

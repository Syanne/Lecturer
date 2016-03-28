using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Lecturer.Data.Entities;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lecturer.Data.Processor;
using System.Xml.Linq;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for PersonalData.xaml
    /// </summary>
    public partial class UserDataPage : Page
    {
        List<UserData> uData;
        int SelectedIndex;
        XMLProcessor xProc;

        public UserDataPage()
        {
            InitializeComponent();
            uData = new List<UserData>();
            xProc = new XMLProcessor("settings.xml");

            PrepareUserData();
        }

        private void PrepareUserData()
        {
            var root = xProc.PersonalData.Root;

            //имя
            uData.Add(new UserData
            {
                Key = "name",
                Value = root.Attribute("name").Value,
                Title = "Ім'я:",
                Tag = uData.Count,
                CanChangeValue = Visibility.Visible
            });

            //фамилия
            uData.Add(new UserData
            {
                Key = "surname",
                Value = root.Attribute("surname").Value,
                Title = "Прізвище:",
                Tag = uData.Count,
                CanChangeValue = Visibility.Visible
            });

            //спеціальність
            uData.Add(new UserData
            {
                Key = "specialityName",
                Value = root.Attribute("specialityName").Value,
                Title = "Напрям підготовки:",
                Tag = uData.Count,
                CanChangeValue = Visibility.Collapsed
            });

            //семестр
            uData.Add(new UserData
            {
                Key = "semester",
                Value = root.Attribute("semester").Value,
                Title = "Семестр:",
                Tag = uData.Count,
                CanChangeValue = Visibility.Visible
            });

            //розташування 
            uData.Add(new UserData
            {
                Key = "location",
                Value = root.Attribute("location").Value,
                Title = "Розташування сховища:",
                Tag = uData.Count,
                CanChangeValue = Visibility.Visible
            });

            lvPersonalData.ItemsSource = uData;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnOnListView_Click(object sender, RoutedEventArgs e)
        {
            SelectedIndex = (int)(sender as Button).Tag;

            var instance = uData[SelectedIndex];
            InputDialog inputDialog = new InputDialog(instance.Title, instance.Value);

            if (SelectedIndex != uData.Count - 1)
            {
                if (inputDialog.ShowDialog() == true)
                {
                    if (inputDialog.Answer != "" && inputDialog.Answer != instance.Value)
                    {
                        var root = xProc.PersonalData.Root;

                        root.Attribute(instance.Key).Value = inputDialog.Answer;
                        xProc.SaveDocument();
                        instance.Value = inputDialog.Answer;

                        lvPersonalData.ItemsSource = null;
                        lvPersonalData.ItemsSource = uData;
                    }
                }
            }
            else
            {
                System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();

                folderDlg.ShowNewFolderButton = true;

                // Show the FolderBrowserDialog
                System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    var root = xProc.PersonalData.Root;

                    root.Attribute(instance.Key).Value = folderDlg.SelectedPath;
                    xProc.SaveDocument();
                    instance.Value = inputDialog.Answer;
                    Cource.MyCource.RootFolderPath = System.IO.Path.Combine(folderDlg.SelectedPath, Cource.MyCource.Semester);

                    instance.Value = folderDlg.SelectedPath;

                    lvPersonalData.ItemsSource = null;
                    lvPersonalData.ItemsSource = uData;
                }
            }
        }
    }
}

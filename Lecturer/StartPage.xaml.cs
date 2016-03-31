using Lecturer.Data.Processor;
using Lecturer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        List<Institute> inses;
        public StartPage()
        {
            InitializeComponent();


            GetUniversityList();

            
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (comboSemester.SelectedIndex != -1 && Folder.Text != "")
            {
                ProcessUserFile();
            }

        }

        private void GetUniversityList()
        {
            inses = new List<Institute>();
            try
            { 
                XMLProcessor xProc = new XMLProcessor("University.xml");

                //список всех институтов и факультетов
                foreach (var item in xProc.PersonalData.Root.Elements("univertsity"))
                {
                    Institute ins = new Institute
                    {
                        Name = item.Attribute("name").Value,
                        FolderName = item.Attribute("folder").Value,
                        Specialities = new List<Speciality>()
                    };

                    //все специальности
                    foreach (var spec in item.Elements("speiality"))
                    {
                        Speciality sp = new Speciality
                        {
                            Name = spec.Attribute("name").Value,
                            Code = spec.Attribute("code").Value,
                            FolderName = spec.Attribute("folder").Value,
                            Cources = new List<int>()
                        };

                        //курсы
                        int count = Convert.ToInt32(spec.Attribute("last").Value);
                        int start = Convert.ToInt32(spec.Attribute("first").Value);
                        for (int i = start; i <= count; i++)
                        {
                            sp.Cources.Add(i);
                        }

                        ins.Specialities.Add(sp);
                    }

                    inses.Add(ins);
                }

                comboIns.ItemsSource = inses;
            }
            catch
            {

            }
        }

        private void ProcessUserFile()
        {
            XMLProcessor processor = new XMLProcessor("settings.xml");
            if (processor.PersonalData == null)
            {
                Cource.MyCource.Semester = comboSemester.SelectedValue.ToString();
                Cource.MyCource.GroupName = comboCource.SelectedValue.ToString() + " " + (comboSpec.SelectedValue as Speciality).Code;
                Cource.MyCource.SpecialityCode = (comboSpec.SelectedValue as Speciality).FolderName;
                Cource.MyCource.SpecialityName = (comboSpec.SelectedValue as Speciality).Name;


                //создание файла с настройками
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                dictionary.Add("name", "");
                dictionary.Add("surname", "");
                dictionary.Add("semester", Cource.MyCource.Semester);
                dictionary.Add("location", Cource.MyCource.RootFolderPath);
                dictionary.Add("specialityCode", Cource.MyCource.SpecialityCode);
                dictionary.Add("specialityName", Cource.MyCource.SpecialityName);

                processor.CreateSettingsFile(dictionary);


                string subfolder = (comboIns.SelectedItem as Institute).FolderName + @"/"
                                    + Cource.MyCource.SpecialityCode + @"/"
                                    + Cource.MyCource.Semester + @"/";
                string[] ext = { "zip" };

                //act
                string path = StorageProcessor.TryGetFileByFTP(subfolder, Cource.MyCource.RootFolderPath, ext);
                bool flag = StorageProcessor.ProcessZipFile(path, Cource.MyCource.RootFolderPath);
                
                
                StorageProcessor.ProcessSchedule(comboIns.SelectedItem as Institute);
                processor.FillSemester();
                Cource.MyCource.RootFolderPath = System.IO.Path.Combine(Cource.MyCource.RootFolderPath, Cource.MyCource.Semester);
            }
            

            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
        }



        private void Folder_GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = true;

            // Show the FolderBrowserDialog
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Folder.Text = folderDlg.SelectedPath;
                Cource.MyCource.RootFolderPath = StorageProcessor.CreateDirectory(folderDlg.SelectedPath, "Лектор");
            }
            //else Folder.Text = "";
        }

        private void comboIns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboSpec.IsEnabled == true)
            {
                comboCource.ItemsSource = null;
                comboCource.IsEnabled = false;

                comboSemester.ItemsSource = null;
                comboSemester.IsEnabled = false;
            }

            comboSpec.ItemsSource = (comboIns.SelectedItem as Institute).Specialities;
            comboSpec.IsEnabled = true;
        }

        private void comboSpec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboCource.IsEnabled == true)
            {
                comboSemester.ItemsSource = null;
                comboSemester.IsEnabled = false;
            }

            comboCource.ItemsSource = (comboSpec.SelectedItem as Speciality).Cources;
            comboCource.IsEnabled = true;
        }

        private void comboCource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<int> ListOfIntegers;
            int switcher = (int)comboCource.SelectedItem;
            switch (switcher)
            {
                case 1: ListOfIntegers = new List<int> { 1, 2 }; break;
                case 2: ListOfIntegers = new List<int> { 3, 4 }; break;
                case 3: ListOfIntegers = new List<int> { 5, 6 }; break;
                case 4: ListOfIntegers = new List<int> { 7, 8 }; break;
                default: ListOfIntegers = new List<int> { 9, 10, 11 }; break;
            }

            comboSemester.ItemsSource = ListOfIntegers;
            comboSemester.IsEnabled = true;
        }

        private List<int> mListOfIntegers = new List<int>();
    }
}
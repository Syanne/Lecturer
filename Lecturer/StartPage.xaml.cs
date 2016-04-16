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
        List<Department> Departments { get; set; }

        #region слушатели
        public StartPage()
        {
            InitializeComponent();

            GetUniversityList();            
        }
        
        
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            loadingGrid.Visibility = Visibility.Visible;
            if (comboSemester.SelectedIndex != -1 && Folder.Text != "")
            {
                ProcessUserFile();
            }

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

            comboSpec.ItemsSource = (comboIns.SelectedItem as Department).Specialities;
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

        #endregion


        /// <summary>
        /// Получает список подразделений
        /// </summary>
        private void GetUniversityList()
        {
            Departments = new List<Department>();
            try
            {
                XMLProcessor xProc = new XMLProcessor("University.xml");

                //список всех институтов и факультетов
                foreach (var item in xProc.XFile.Root.Elements("univertsity"))
                {
                    Department ins = new Department
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

                    Departments.Add(ins);
                }

                comboIns.ItemsSource = Departments;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Скачивание данных с сервера и сохранение в файл с личными данными/на диск
        /// </summary>
        private async void ProcessUserFile()
        {
            XMLProcessor processor = new XMLProcessor("settings.xml");
            if (processor.XFile == null)
            {
                Cource.MyCource.Semester = comboSemester.SelectedValue.ToString();
                Cource.MyCource.GroupName = comboCource.SelectedValue.ToString() + " " + (comboSpec.SelectedValue as Speciality).Code;
                Cource.MyCource.SpecialityCode = (comboSpec.SelectedValue as Speciality).FolderName;
                Cource.MyCource.SpecialityName = (comboSpec.SelectedValue as Speciality).Name;
                Cource.MyCource.InstituteCode = (comboIns.SelectedValue as Department).FolderName;
                Cource.MyCource.CourceNumber = comboCource.SelectedValue.ToString();


                //создание файла с настройками
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                dictionary.Add("name", "");
                dictionary.Add("surname", "");
                dictionary.Add("location", Cource.MyCource.RootFolderPath);
                dictionary.Add("institute", Cource.MyCource.InstituteCode);
                dictionary.Add("specialityCode", Cource.MyCource.SpecialityCode);
                dictionary.Add("specialityName", Cource.MyCource.SpecialityName);
                dictionary.Add("courceNumber", Cource.MyCource.CourceNumber);
                dictionary.Add("semester", Cource.MyCource.Semester);

                //загрузка данных
                await StorageProcessor.GetSemesterFilesAsync();

                processor.CreateSettingsFile(dictionary);
                processor.WriteSemester();
            }

            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CourcePage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
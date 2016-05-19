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
            if (comboCourse.SelectedIndex > -1 && Folder.Text != "")
            {
                loadingGrid.Visibility = Visibility.Visible;
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
                Course.MyCourse.RootFolderPath = StorageProcessor.CreateDirectory(folderDlg.SelectedPath, "Лектор");
            }
        }

        private void comboIns_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboSpec.IsEnabled == true)
            {
                comboCourse.ItemsSource = null;
                comboCourse.IsEnabled = false;
            }

            comboSpec.ItemsSource = (comboIns.SelectedItem as Department).Specialities;
            comboSpec.IsEnabled = true;
        }

        private void comboSpec_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            comboCourse.ItemsSource = (comboSpec.SelectedItem as Speciality).Courses;
            comboCourse.IsEnabled = true;
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
                            Courses = new List<int>()
                        };

                        //курсы
                        int count = Convert.ToInt32(spec.Attribute("last").Value);
                        int start = Convert.ToInt32(spec.Attribute("first").Value);
                        for (int i = start; i <= count; i++)
                        {
                            sp.Courses.Add(i);
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
                //определить семестр
                int courceValue = Convert.ToInt32(comboCourse.SelectedValue);
                int semester = 0;
                if (DateTime.Now.Month > 1 && DateTime.Now.Month <= 5)
                    semester = courceValue * 2;
                else if (DateTime.Now.Month >= 9)
                    semester = courceValue * 2 - 1;


                Course.MyCourse.Semester = semester.ToString();
                Course.MyCourse.GroupName = comboCourse.SelectedValue.ToString() + " " + (comboSpec.SelectedValue as Speciality).Code;
                Course.MyCourse.SpecialityCode = (comboSpec.SelectedValue as Speciality).FolderName;
                Course.MyCourse.SpecialityName = (comboSpec.SelectedValue as Speciality).Name;
                Course.MyCourse.InstituteCode = (comboIns.SelectedValue as Department).FolderName;
                Course.MyCourse.CourseNumber = comboCourse.SelectedValue.ToString();


                //создание файла с настройками
                Dictionary<string, string> dictionary = new Dictionary<string, string>();

                dictionary.Add("name", "");
                dictionary.Add("surname", "");
                dictionary.Add("location", Course.MyCourse.RootFolderPath);
                dictionary.Add("institute", Course.MyCourse.InstituteCode);
                dictionary.Add("specialityCode", Course.MyCourse.SpecialityCode);
                dictionary.Add("specialityName", Course.MyCourse.SpecialityName);
                dictionary.Add("courceNumber", Course.MyCourse.CourseNumber);
                dictionary.Add("semester", Course.MyCourse.Semester);

                //загрузка данных
                await StorageProcessor.GetSemesterFilesAsync();

                processor.CreateSettingsFile(dictionary);
                processor.WriteSemester();
            }

            NavigationService nav = NavigationService.GetNavigationService(this);
            nav.Navigate(new Uri("CoursePage.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
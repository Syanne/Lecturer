using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Lecturer.TestCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Questions> ques;
        List<TestItem> testItems;

        public MainWindow()
        {
            InitializeComponent();
            ques = new List<Questions>();
            testItems = new List<TestItem>();

            for (int i = 0; i < 2; i++)
                CreateItem();

            ansList.ItemsSource = testItems;
            labelQuesNumber.Content = "Питання №" + (ques.Count + 1);
        }

        private void CreateItem()
        {
            TestItem test = new TestItem
            {
                Answer = "",
                IsTrue = false, 
                Tag = testItems.Count - 1
            };

            testItems.Add(test);
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int points = Convert.ToInt32(minPoints.Text);

                if (points < ques.Count && points > 0)
                    if (ques.Count >= 2 && tbTestName.Text != "")
                    {
                        Dictionary<string, string> dict = new Dictionary<string, string>();
                        dict.Add("testName", tbTestName.Text);
                        dict.Add("minPoints", minPoints.Text);


                        XMLProcessor xProc = new XMLProcessor(PreparePath());
                        xProc.CreateTestFile(dict, ques);                        
                    }
            }
            catch (Exception ex)
            {

            }
        }

        private string PreparePath()
        {
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();

            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.
            System.Windows.Forms.DialogResult result = folderDlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return System.IO.Path.Combine(folderDlg.SelectedPath, "quiz.xml");
            }
            else return "quiz.xml";
        }

        private void btnAddOption_Click(object sender, RoutedEventArgs e)
        {
            CreateItem();
            ansList.ItemsSource = null;
            ansList.ItemsSource = testItems;
        }

        private void btnAddQuestion_Click(object sender, RoutedEventArgs e)
        {
            //проверяем ответы 
            bool flagAns = true;
            int trues = 0;
            foreach(var item in testItems)
            {
                //ищем ответы с пустым текстом
                if(item.Answer == "")
                {
                    flagAns = false;
                    break;
                }
                //проверяем, есть ли хоть один правильный ответ
                if (item.IsTrue == true)
                    trues += 1;
            }

            //проверяем все условия
            if(flagAns == true && tbQuestion.Text != "" && trues > 0)
            {
                ques.Add(new Questions
                {
                    QuestionText = tbQuestion.Text,
                    MyTest = testItems,
                    TrueCount = trues
                });

                //очищаем поля
                testItems = new List<TestItem>();
                for (int i = 0; i < 2; i++)
                    CreateItem();

                ansList.ItemsSource = testItems;
                labelQuesNumber.Content = "Питання №" + (ques.Count + 1);
                tbQuestion.Text = "";
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (testItems.Count > 2)
            {
                int tag = (int)(sender as Button).Tag;

                testItems.RemoveAt(tag + 1);
                ansList.ItemsSource = null;
                ansList.ItemsSource = testItems;
            }
        }
    }
}

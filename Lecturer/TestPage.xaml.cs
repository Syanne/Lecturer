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
    /// Interaction logic for TestPage.xaml
    /// </summary>
    public partial class TestPage : Page
    {
        int minPoints;
        int qCount;

        public TestPage()
        {
            InitializeComponent();

            PrepareTest();
        }

        private void PrepareTest()
        {
            //mock
            XMLProcessor x = new XMLProcessor("quiz.xml");
            Quiz quiz = x.ReadQuizFile();

            //------

            QuizName.Text = quiz.TestName;
            for (int i = 0; i < quiz.Questions.Count; i++)
            {
                StackPanel panel = new StackPanel();
                TextBlock tb = new TextBlock
                {
                    Text = quiz.Questions[i].Text,
                    Style = (Style)this.Resources["tbQuestionStyle"]
                };
                panel.Children.Add(tb);

                for (int j = 0; j < quiz.Questions[i].Answers.Count; j++)
                {
                    if (quiz.Questions[i].IsOneTrue == false)
                        panel.Children.Add(CreateCheckBox(quiz.Questions[i].Answers[j],
                            quiz.Questions[i].Values[j]));
                    else panel.Children.Add(CreateRadioButton(quiz.Questions[i].Answers[j],
                            quiz.Questions[i].Values[j]));
                }

                TestPanel.Children.Add(panel);
            }
        }

        private RadioButton CreateRadioButton(string text, string tag)
        {
            return new RadioButton
            {
                Content = text,
                Tag = tag,
                IsChecked = false,
                Style = (Style)this.Resources["rbStyle"]
            };
        }
        private CheckBox CreateCheckBox(string text, string tag)
        {
            return new CheckBox
            {
                Content = text,
                Tag = tag,
                IsChecked = false,
                Style = (Style)this.Resources["cbStyle"]
            };
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

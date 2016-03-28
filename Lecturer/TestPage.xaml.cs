using Lecturer.Data.Processor;
using Lecturer.Data.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Lecturer
{
    /// <summary>
    /// Interaction logic for TestPage.xaml
    /// </summary>
    public partial class TestPage : Page
    {
        Quiz quiz;
        int Points;
        public TestPage()
        {
            InitializeComponent();
            Points = 0;
            PrepareTest();
        }

        private void PrepareTest()
        {
            XMLProcessor x = new XMLProcessor(Cource.MyCource.SelectedSubject.SelectedTopic.TestUri);
            quiz = x.ReadQuizFile();
            

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
                panel.Tag = i;

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
            Points = 0;
            int? isAllHasAnswer = 0;

            //просматриваем все вопросы
            foreach (var panel in TestPanel.Children)
                {
                    int tag = Convert.ToInt32((panel as StackPanel).Tag);
                    // var type = (quiz.Questions[tag].IsOneTrue == true) ? typeof(RadioButton) : typeof(CheckBox);
                    bool flag = true;
                    isAllHasAnswer = 0;

                //просматриваем все ответы 
                    foreach (var children in (panel as StackPanel).Children)
                    {
                        if (children.GetType() == typeof(RadioButton))
                        {
                            if ((children as RadioButton).IsChecked == false)
                                isAllHasAnswer += 1;

                            if ((children as RadioButton).IsChecked == Convert.ToBoolean((children as RadioButton).Tag))
                            {
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                            }
                        }
                        else if (children.GetType() == typeof(CheckBox))
                        {
                            if ((children as CheckBox).IsChecked == false)
                                isAllHasAnswer += 1;

                            if ((children as CheckBox).IsChecked == Convert.ToBoolean((children as CheckBox).Tag))
                            {
                                flag = true;
                            }
                            else
                            {
                                flag = false;
                            }
                        }

                    }
                //если пользователь не ответил ни на один вопрос - выбрасываем предупреждение
                if (isAllHasAnswer == (panel as StackPanel).Children.Count - 1)
                {
                    isAllHasAnswer = null;
                    if (MessageBox.Show("Ви відповіли не на усі питання!", "Результат") == MessageBoxResult.OK)
                        break;
                }

                    if (flag == true)
                        Points += 1;
                }

            //Результат прохождения теста
            if (isAllHasAnswer != null)
            {
                string message = String.Format("Ваш результат складає {0} з {1} \n{2}",
                    Points.ToString(),
                    quiz.Questions.Count,
                    (quiz.MinPoints > Points) ? "Ви не пройшли тест" : "Ви пройшли тест");


                if (MessageBox.Show(message, "Результат") == MessageBoxResult.OK)
                {
                    if (quiz.MinPoints < Points)
                    {
                        XMLProcessor xProc = new XMLProcessor("settings.xml");
                        xProc.SetTopicStudied();

                        Cource.MyCource.SelectedSubject.SelectedTopic.IsStudied = true;
                    }
                }

                NavigationService nav = NavigationService.GetNavigationService(this);
                Cource.MyCource.SelectedSubject.SelectedTopic = null;
                nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));
            }

        }
    }
}

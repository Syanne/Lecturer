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
        Quiz MainQuiz { get; set; }
        int Points { get; set; }

        public TestPage()
        {
            InitializeComponent();
            Points = 0;
            PrepareTest();
        }

        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            Points = 0;
            int? isAnswered  = null;

            //просматриваем все вопросы
            foreach (var panel in TestPanel.Children)
            {
                int counter = 0;
                isAnswered  = 0;

                //просматриваем все ответы 
                foreach (var child in (panel as StackPanel).Children)
                {
                    if (child.GetType() == typeof(RadioButton))
                    {
                        if ((child as RadioButton).IsChecked == false)
                            isAnswered  += 1;

                        if ((child as RadioButton).IsChecked == Convert.ToBoolean((child as RadioButton).Tag))
                        {
                            counter += 1;
                        }
                    }
                    else if (child.GetType() == typeof(CheckBox))
                    {
                        if ((child as CheckBox).IsChecked == false)
                            isAnswered  += 1;

                        if ((child as CheckBox).IsChecked == Convert.ToBoolean((child as CheckBox).Tag))
                        {
                            counter += 1;
                        }
                    }

                }
                //если пользователь не ответил ни на один вопрос - выбрасываем предупреждение
                if (isAnswered  == (panel as StackPanel).Children.Count - 1)
                {
                    isAnswered  = null;
                    if (MessageBox.Show("Ви відповіли не на усі питання!", "Результат") == MessageBoxResult.OK)
                        break;
                }

                if (counter == (panel as StackPanel).Children.Count - 1)
                    Points += 1;
            }

            //Результат прохождения теста
            if (isAnswered  != null)
            {
                string message = String.Format("Ваш результат складає {0} з {1} \n{2}",
                    Points.ToString(),
                    MainQuiz.Questions.Count,
                    (MainQuiz.MinPoints > Points) ? "Ви не пройшли тест" : "Ви пройшли тест");


                if (MessageBox.Show(message, "Результат") == MessageBoxResult.OK)
                {
                    if (MainQuiz.MinPoints <= Points)
                    {
                        XMLProcessor xProc = new XMLProcessor("settings.xml");
                        xProc.SetTopicStudied();

                        Course.MyCourse.SelectedSubject.SelectedTopic.IsStudied = true;
                    }
                }

                NavigationService nav = NavigationService.GetNavigationService(this);
                Course.MyCourse.SelectedSubject.SelectedTopic = null;
                nav.Navigate(new Uri("SubjectPage.xaml", UriKind.RelativeOrAbsolute));
            }
        }
        

        /// <summary>
        /// Подготовка теста
        /// </summary>
        private void PrepareTest()
        {
            //файл теста
            XMLProcessor x = new XMLProcessor(Course.MyCourse.SelectedSubject.SelectedTopic.TestUri);
            MainQuiz = x.ReadQuizFile();

            if (MainQuiz != null)
            {
                //название теста
                QuizName.Text = MainQuiz.TestName;

                //вопросы
                for (int i = 0; i < MainQuiz.Questions.Count; i++)
                {
                    StackPanel panel = new StackPanel
                    {
                        Margin = new Thickness(5, 5, 5, 10)
                    };

                    //вопрос
                    TextBlock tb = new TextBlock
                    {
                        Text = (i+1) +". "+MainQuiz.Questions[i].Text,
                        Style = (Style)this.Resources["tbQuestionStyle"]
                    };

                    panel.Children.Add(tb);
                    panel.Tag = i;

                    //варианты ответа
                    for (int j = 0; j < MainQuiz.Questions[i].Answers.Count; j++)
                    {
                        if (MainQuiz.Questions[i].IsOneTrue == false)
                            panel.Children.Add(CreateCheckBox(MainQuiz.Questions[i].Answers[j],
                                MainQuiz.Questions[i].Values[j]));
                        else panel.Children.Add(CreateRadioButton(MainQuiz.Questions[i].Answers[j],
                                MainQuiz.Questions[i].Values[j]));
                    }

                    TestPanel.Children.Add(panel);
                }
            }
        }


        /// <summary>
        /// создание радио-кнопки
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="tag">значение (правильный ответ или нет)</param>
        /// <returns>радио-кнопка</returns>
        private RadioButton CreateRadioButton(string text, string tag)
        {
            return new RadioButton
            {
                Content = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14
                },
                Tag = tag,
                IsChecked = false,
                Style = (Style)this.Resources["rbStyle"]
            };
        }


        /// <summary>
        /// создание чекбокса
        /// </summary>
        /// <param name="text">текст</param>
        /// <param name="tag">значение (правильный ответ или нет)</param>
        /// <returns>чекбокс</returns>
        private CheckBox CreateCheckBox(string text, string tag)
        {
            return new CheckBox
            {
                Content = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14
                },
                Tag = tag,
                IsChecked = false,
                Style = (Style)this.Resources["cbStyle"]
            };
        }
    }
}

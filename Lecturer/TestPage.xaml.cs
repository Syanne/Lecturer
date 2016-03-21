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
            int quesInTest = 4;
            qCount = 6;
            minPoints = 5;
            //------

            for(int i = 0; i < qCount; i++)
            {
                StackPanel panel = new StackPanel();
                TextBlock tb = new TextBlock
                {
                    Text = "question number " + i,
                    Style = (Style)this.Resources["tbQuestionStyle"]
                };
                panel.Children.Add(tb);

                for(int j = 0; j< quesInTest; j++)
                {
                    panel.Children.Add(CreateCheckBox("content" + j, "false"));
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

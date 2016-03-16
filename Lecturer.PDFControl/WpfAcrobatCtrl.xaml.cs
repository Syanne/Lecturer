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

namespace Lecturer.PDFControl
{
    /// <summary>
    /// Interaction logic for WpfAcrobatCtrl.xaml
    /// </summary>
    public partial class WpfAcrobatCtrl : UserControl
    {
        public static readonly DependencyProperty FilePathProperty
            = DependencyProperty.Register("FilePath", typeof(string), typeof(WpfAcrobatCtrl), (PropertyMetadata)new FrameworkPropertyMetadata((object)null, new PropertyChangedCallback(WpfAcrobatCtrl.FilePathChanged)));


        private string _filePath = string.Empty;
        private PdfControl _customAcrobatCtrl;


        private static void FilePathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WpfAcrobatCtrl)d).FilePathChanged((string)e.OldValue, (string)e.NewValue);
            CommandManager.InvalidateRequerySuggested();
        }

        public WpfAcrobatCtrl()
        {
            InitializeComponent();
            _customAcrobatCtrl = new PdfControl();
            wpfWindowsFormsHostCtrl.Child = _customAcrobatCtrl;

        }

        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                this.SetValue(FilePathProperty, value);
            }
        }


        private void FilePathChanged(string oldFilePath, string newFilePath)
        {
            _filePath = newFilePath;
            _customAcrobatCtrl.LoadFile(_filePath);
        }
    }
}

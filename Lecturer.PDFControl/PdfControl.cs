using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lecturer.PDFControl
{
    public partial class PdfControl : UserControl
    {
        public PdfControl()
        {
            InitializeComponent();
        }

        private AxAcroPDFLib.AxAcroPDF AdobeAcrobatPDfControl
		{
			get
			{
				return this.axAcroPDF;
			}
		}
 
		public void LoadFile(string pdfFilePath)
		{
			AdobeAcrobatPDfControl.LoadFile(pdfFilePath);
		}
    }
}

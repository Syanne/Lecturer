using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.Text;

namespace Lecturer.Data.Processor
{
    public class DocxProcessor
    {
        /// <summary>
        /// Обрабатываем параграфы в файле
        /// </summary>
        /// <returns>Текст документа, содержащийся в параграфах</returns>
        public StringBuilder ProcessParagraph()
        {
            WordprocessingDocument wordProcessingDocument = WordprocessingDocument.Open("attempt.docx", false, new OpenSettings());
            StringBuilder wordDocumentText = new StringBuilder();
            IEnumerable<Paragraph> paragraphElements =
                wordProcessingDocument.MainDocumentPart.Document.Body.Descendants<Paragraph>();

            foreach (Paragraph p in paragraphElements)
            {
                IEnumerable<Text> textElements = p.Descendants<Text>();

                foreach (Text t in textElements)
                {
                    wordDocumentText.Append(t.Text);
                }

                wordDocumentText.AppendLine();
            }

            return wordDocumentText;
        }
    }
}

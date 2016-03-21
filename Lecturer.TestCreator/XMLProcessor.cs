using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Lecturer.TestCreator
{
    public class XMLProcessor
    {
        private string Path { get; set; }
        public XDocument PersonalData { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="pathString">путь к файлу</param>
        public XMLProcessor(string pathString)
        {
            Path = pathString;

            try
            {
                PersonalData = XDocument.Load(Path);
            }
            catch
            {
                PersonalData = null;
            }
        }
        

        #region чтение
        /// <summary>
        /// Читает содержимое тега
        /// </summary>
        /// <param name="isRootParent">является корневой элемент единственным предком</param>
        /// <param name="parent">родительский тег</param>
        /// <param name="key">тег, значение которого нужно прочесть</param>
        /// <returns>значение</returns>
        public string ReadAttributeValue(XElement element, string key)
        {
            //try
            //{
            //    if (isRootParent == true)
            //        return PersonalData.Root.Element(key).Value;
            //    else
            //        return PersonalData.Root.Element(parent).Element(key).Value;
            //}
            //catch
            //{
            //    return "";
            //}
            return "";
        }

        #endregion


        #region запись


        /// <summary>
        /// Saves changed personal data
        /// </summary>
        private void SaveDocument()
        {
            PersonalData.Save(Path, SaveOptions.OmitDuplicateNamespaces);
        }

        /// <summary>
        /// Создание файла личных настроек
        /// </summary>
        /// <param name="dictionary">словарь значений, которые будут записаны в файл</param>
        public void CreateTestFile(Dictionary<string, string> dictionary, List<Questions> ques)
        {
            try
            {
                PersonalData = new XDocument();
                //root
                PersonalData = new XDocument(new XElement("root", PersonalData.Root));
                
                //first parent
                using (XmlWriter writer = PersonalData.Root.CreateWriter())
                {
                    for (int i = 0; i < dictionary.Count; i++)
                    {
                        SetAttribute(writer, dictionary.Keys.ElementAt(i), dictionary.Values.ElementAt(i));
                    }                    
                }


                //first parent
                using (XmlWriter writer = PersonalData.Root.CreateWriter())
                {
                    for (int i = 0; i < ques.Count; i++)
                    {
                        GenerateElement(writer, "question", "");
                    }
                }

                foreach (var q in PersonalData.Root.Elements("question"))
                {
                    int counter = 0;
                    using (XmlWriter writer = q.CreateWriter())
                    {
                        SetAttribute(writer, "text", ques[counter].QuestionText);
                        SetAttribute(writer, "trues", ques[counter].TrueCount.ToString());
                    }

                    using (XmlWriter writer = q.CreateWriter())
                    {
                        for (int i = 0; i < ques[counter].MyTest.Count; i++)
                        {
                            GenerateElement(writer, "ans", "");
                        }
                    }

                    for (int i = 0; i < ques[counter].MyTest.Count; i++)
                    {
                        using (XmlWriter writer = q.Elements().ElementAt(i).CreateWriter())
                        {
                            SetAttribute(writer, "text", ques[counter].MyTest[i].Answer);
                            SetAttribute(writer, "value", ques[counter].MyTest[i].IsTrue.ToString());
                        }
                    }
                    counter += 1;
                }

                //save changes
                SaveDocument();
            }
            catch (Exception e)
            {

            }
        }
        
        #endregion


        #region обработка элементов

        /// <summary>
        /// Генерация тега и его содержимого
        /// </summary>
        /// <param name="writer">экземпляр XmlWriter</param>
        /// <param name="name">название тега (ключ)</param>
        /// <param name="content">содержимое тега (значение)</param>
        private void GenerateElement(XmlWriter writer, string name, string content)
        {
            writer.WriteStartElement(name);
            writer.WriteString(content);
            writer.WriteEndElement();
        }



        /// <summary>
        /// Добавление аттрибута
        /// </summary>
        /// <param name="writer">Поток</param>
        /// <param name="key">название аттрибута</param>
        /// <param name="value">значение аттрибута</param>
        private void SetAttribute(XmlWriter writer, string key, string value)
        {
            writer.WriteStartAttribute(key);
            writer.WriteString(value);
            writer.WriteEndAttribute();
        }


        /// <summary>
        /// Изменяет содержимое элемента
        /// </summary>
        /// <param name="newValue">новое значение</param>
        public void EditValue(XElement element, string newValue)
        {
            try
            {
                element.Value = newValue;

                SaveDocument();
            }
            catch
            {
            }
        }


        /// <summary>
        /// Удаление элемента
        /// </summary>
        /// <param name="isRootParent">является корневой элемент единственным предком</param>
        /// <param name="parent">родительский тег</param>
        /// <param name="key">тег, который нужно удалить</param>
        public void RemoveElement(XElement parent, string key, string value)
        {
            try
            {
                parent.Elements(key).Where(elem => elem.Value == value).Remove();

                SaveDocument();
            }
            catch
            {
            }
        }

        #endregion


    }

}
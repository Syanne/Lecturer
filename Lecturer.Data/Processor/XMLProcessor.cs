using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Lecturer.Data.DataProcessor
{
    public class XMLProcessor
    {
        private string Path { get; set; }
        private XDocument PersonalData { get; set; }

        public XMLProcessor(string pathString)
        {
            Path = pathString;

            try
            {
                PersonalData = XDocument.Load(Path);
            }
            catch
            {
                PersonalData = new XDocument();
            }
        }
        
        /// <summary>
        /// Saves changed personal data
        /// </summary>
        public void SaveDocument()
        {
            PersonalData.Save(Path, SaveOptions.OmitDuplicateNamespaces);
        }
        

        /// <summary>
        /// Создание файла личных настроек
        /// </summary>
        /// <param name="dictionary">словарь значений, которые будут записаны в файл</param>
        public void CreateSettingsFile(Dictionary<string, string> dictionary)
        {
            try
            {
                //root
                PersonalData = new XDocument(new XElement("data", PersonalData.Root));
                
                //first parent
                using (XmlWriter writer = PersonalData.Root.CreateWriter())
                {
                    GenerateElement(writer, "userdata", "");
                }

                //userdata
                using (XmlWriter innerWriter = PersonalData.Root.Element("userdata").CreateWriter())
                {
                    for (int i = 0; i < dictionary.Count; i++)
                    {
                        GenerateElement(innerWriter, dictionary.Keys.ElementAt(i), dictionary.Values.ElementAt(i));
                    }                    
                }                

                //save changes
                SaveDocument();
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Генерация тега и его содержимого
        /// </summary>
        /// <param name="writer">экземпляр XmlWriter</param>
        /// <param name="name">название тега (ключ)</param>
        /// <param name="content">содержимое тега (значение)</param>
        public void GenerateElement(XmlWriter writer, string name, string content)
        {
            writer.WriteStartElement(name);
            writer.WriteString(content);
            writer.WriteEndElement();
        }
        

        /// <summary>
        /// Читает содержимое тега
        /// </summary>
        /// <param name="isRootParent">является корневой элемент единственным предком</param>
        /// <param name="parent">родительский тег</param>
        /// <param name="key">тег, значение которого нужно прочесть</param>
        /// <returns>значение</returns>
        public string ReadValue(bool isRootParent, string parent, string key)
        {
            try
            {
                if (isRootParent == true)
                    return PersonalData.Root.Element(key).Value;
                else
                    return PersonalData.Root.Element(parent).Element(key).Value;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Изменяет содержимое тега
        /// </summary>
        /// <param name="isRootParent">является корневой элемент единственным предком</param>
        /// <param name="parent">родительский тег</param>
        /// <param name="key">тег, значение которого нужно прочесть</param>
        /// <param name="newValue">новое значение</param>
        public void EditValue(bool isRootParent, string parent, string key, string newValue)
        {
            try
            {
                if (isRootParent == true)
                    PersonalData.Root.Element(key).Value = newValue;
                else
                    PersonalData.Root.Element(parent).Element(key).Value = newValue;

                SaveDocument();
            }
            catch
            {
            }
        }


        /// <summary>
        /// Читает содержимое тега
        /// </summary>
        /// <param name="isRootParent">является корневой элемент единственным предком</param>
        /// <param name="parent">родительский тег</param>
        /// <param name="key">тег, который нужно удалить</param>
        public void RemoveElement(bool isRootParent, string parent, string key)
        {
            try
            {
                if (isRootParent == true)
                    PersonalData.Root.Element(key).Remove();
                else
                    PersonalData.Root.Element(parent).Element(key).Remove();

                SaveDocument();
            }
            catch
            {
            }
        }
    }

}
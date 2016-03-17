using Lecturer.Data.Entities;
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
        /// Заполняет структуру данными о списке 
        /// дисциплин на семестр
        /// </summary>
        /// <returns></returns>
        public List<Subject> GetSubjectList()
        {
            List<Subject> subj = new List<Subject>();

            try
            {
                //текущий семестр
                string semNumber = PersonalData.Root.Attribute("semester").Value;

                //список дисциплин в семестре
                var list = PersonalData.
                                Root.
                                Elements("semester").
                                Where(sem => sem.Attribute("number").Value == semNumber);

                foreach (var subject in list.Elements("subject"))
                {
                    subj.Add(new Subject
                    {
                        Name = subject.Attribute("name").Value,
                        Hours = subject.Attribute("hours").Value,
                        Teacher = subject.Attribute("teacher").Value
                    });
                }
            }
            catch
            {
                subj = null;
            }

            return subj;
        }


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
        public void CreateSettingsFile(Dictionary<string, string> dictionary)
        {
            try
            {

                PersonalData = new XDocument();
                //root
                PersonalData = new XDocument(new XElement("data", PersonalData.Root));
                
                //first parent
                using (XmlWriter writer = PersonalData.Root.CreateWriter())
                {
                    for (int i = 0; i < dictionary.Count; i++)
                    {
                        SetAttribute(writer, dictionary.Keys.ElementAt(i), dictionary.Values.ElementAt(i));
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
        /// заполняем файл с личными данными 
        /// списком дисциплин
        /// </summary>
        public void FillSemester()
        {
            //создаем корневой элемент данного семестра
            using (XmlWriter writer = PersonalData.Root.CreateWriter())
            {
                GenerateElement(writer, "semester", "");
            }

            //создаем 
            var semester = PersonalData.Root.Elements("semester").LastOrDefault();
            using (XmlWriter writer = semester.CreateWriter())
            {
                SetAttribute(writer, "number", Cource.MyCource.Semester);
            }
            using (XmlWriter writer = semester.CreateWriter())
            {
                foreach (var subj in Cource.MyCource.Subjects)
                    GenerateElement(writer, "subject", "");
            }


            for (int i = 0; i < Cource.MyCource.Subjects.Count; i++)
            {
                var subj = Cource.MyCource.Subjects[i];
                using (XmlWriter innerWriter = semester.Elements("subject").ElementAt(i).CreateWriter())
                {
                    SetAttribute(innerWriter, "name", subj.Name);
                    SetAttribute(innerWriter, "hours", subj.Hours);
                    SetAttribute(innerWriter, "teacher", subj.Teacher);
                }
            }
            

            ////TODO список дисциплин
            //foreach (Topic topic in subj.Topics)
            //{
            //    GenerateElement(innerWriter, topic.Name, )
            //            }

            SaveDocument();
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
using Lecturer.Data.Entities;
using Lecturer.TestCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Lecturer.Data.Processor
{
    public class XMLProcessor
    {
        private string Path { get; set; }
        public XDocument XFile { get; set; }

        /// <summary>
        /// Конструктор, загружающий xml-файл
        /// </summary>
        /// <param name="pathString">путь к файлу</param>
        public XMLProcessor(string pathString)
        {
            Path = pathString;

            try
            {
                XFile = XDocument.Load(Path);
            }
            catch
            {
                XFile = null;
            }
        }
        
        /// <summary>
        /// загрузка файла настроек с инициализацией сущности Cource
        /// </summary>
        public XMLProcessor()
        {
            try
            {
                XFile = XDocument.Load("settings.xml");
                var root = XFile.Root;

                Cource.MyCource.Subjects = new List<Subject>();
                Cource.MyCource.Semester = root.Attribute("semester").Value;
                Cource.MyCource.CourceNumber = root.Attribute("courceNumber").Value;
                Cource.MyCource.Subjects = GetSubjectList();
                Cource.MyCource.InstituteCode = root.Attribute("institute").Value;
                Cource.MyCource.SpecialityCode = root.Attribute("specialityCode").Value;
                Cource.MyCource.SpecialityName = root.Attribute("specialityName").Value;
                Cource.MyCource.RootFolderPath = System.IO.Path.Combine(root.Attribute("location").Value);
            }
            catch
            {
                XFile = null;
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

            try
            {
                List<Subject> subj = new List<Subject>();

                //список дисциплин в семестре
                var list = XFile.
                                Root.
                                Elements("semester").
                                Where(sem => sem.Attribute("number").Value == Cource.MyCource.Semester);

                //заполняем список дисциплин
                foreach (var subject in list.Elements("subject"))
                {
                    subj.Add(new Subject
                    {
                        Name = subject.Attribute("name").Value,
                        Hours = subject.Attribute("hours").Value,
                        Teacher = subject.Attribute("teacher").Value
                    });
                }

                return subj;
            }
            catch
            {
                return null;
            }
            
        }

        /// <summary>
        /// Ием список тем в файле
        /// </summary>
        /// <param name="subjectName">Название дисциплины</param>
        /// <returns>список тем</returns>
        public List<Topic> GetTopicList(string subjectName)
        {
            try {
                List<Topic> topics = new List<Topic>();
                var items = XFile.Root.Elements("semester")
                            .FirstOrDefault(elem => elem.Attribute("number").Value == Cource.MyCource.Semester)
                            .Elements("subject")
                            .SingleOrDefault(elem => elem.Attribute("name").Value == subjectName)
                            .Elements("topic");

                if (items == null || items.Count() == 0)
                    throw new Exception();

                foreach (var item in items)
                {
                    topics.Add(new Topic
                    {
                        Name = item.Attribute("name").Value,
                        IsStudied = (item.Attribute("isStudied").Value.ToLower() == "false") ? false : true
                    });
                }

                return topics;
            }
            catch
            {
                return null;
            }
            }
        

        /// <summary>
        /// Чтение и расшифровка файла тестирования
        /// </summary>
        /// <returns>Тест</returns>
        public Quiz ReadQuizFile()
        {
            Quiz quiz = new Quiz();
            try
            {
                //проходной балл и название
                string mp = CryptoProcessor.Decrypt(XFile.Root.Attribute("minPoints").Value);
                quiz.MinPoints = Convert.ToInt32(CryptoProcessor.Decrypt(XFile.Root.Attribute("minPoints").Value));
                quiz.TestName = CryptoProcessor.Decrypt(XFile.Root.Attribute("testName").Value);

                //вопросы
                quiz.Questions = new List<QuizItem>();
                foreach(var question in XFile.Root.Elements("question"))
                {
                    //вопрос
                    QuizItem qItem = new QuizItem();
                    qItem.Text = CryptoProcessor.Decrypt(question.Attribute("text").Value);

                    //варианты ответа и значения
                    qItem.Answers = new List<string>();
                    qItem.Values = new List<string>();
                    int counter = 0;

                    foreach(var ans in question.Elements("ans"))
                    {
                        qItem.Answers.Add(CryptoProcessor.Decrypt(ans.Attribute("text").Value));
                        qItem.Values.Add(CryptoProcessor.Decrypt(ans.Attribute("value").Value));
                        if (qItem.Values.LastOrDefault().ToLower() == "true")
                            counter += 1;
                    }
                    qItem.IsOneTrue = (counter > 1) ? false : true;
                    quiz.Questions.Add(qItem);
                    
                }
            }
            catch(Exception ex)
            {
                quiz = null;
            }

            return quiz;
        }

        /// <summary>
        /// Чтение файла с пользовательскими данными
        /// </summary>
        /// <param name="uData"></param>
        public void PrepareUserData(ref List<UserData> uData)
        {
            //все атрибуты корневого элемента
            var root = XFile.Root.Attributes();

            //определяем заголовок
            foreach (var attrib in root)
            {
                string title;
                switch (attrib.Name.ToString())
                {
                    case "name": title = "Ім'я:"; break;
                    case "surname": title = "Прізвище:"; break;
                    case "specialityName": title = "Напрям підготовки:"; break;
                    case "courceNumber": title = "Курс:"; break;
                    case "location": title = "Розташування сховища:"; break;
                    default: title = null; break;
                }

                //добавляем элемент в коллекцию
                if (title != null)
                {
                    uData.Add(new UserData
                    {
                        Key = attrib.Name.ToString(),
                        Value = attrib.Value,
                        Title = title,
                        Tag = uData.Count,
                        CanChangeValue = (title == "specialityName") ? Visibility.Collapsed : Visibility.Visible
                    });
                }
            }

        }
        #endregion


        #region запись
        /// <summary>
        /// Saves changed personal data
        /// </summary>
        public void SaveDocument()
        {
            XFile.Save(Path, SaveOptions.OmitDuplicateNamespaces);
        }

        /// <summary>
        /// Создание файла личных настроек
        /// </summary>
        /// <param name="dictionary">словарь значений, которые будут записаны в файл</param>
        public void CreateSettingsFile(Dictionary<string, string> dictionary)
        {
            try
            {

                XFile = new XDocument();
                //root
                XFile = new XDocument(new XElement("root", XFile.Root));
                
                //first parent
                using (XmlWriter writer = XFile.Root.CreateWriter())
                {
                    for (int i = 0; i < dictionary.Count; i++)
                    {
                        GenerateAttribute(writer, dictionary.Keys.ElementAt(i), dictionary.Values.ElementAt(i));
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
        /// Установить, что тема изучена
        /// </summary>
        public void SetTopicStudied()
        {
            string parent = Cource.MyCource.SelectedSubject.Name;
            string name = Cource.MyCource.SelectedSubject.SelectedTopic.Name;

            XFile.Root
                .Elements("semester").Where(elem => elem.Attribute("number").Value == Cource.MyCource.Semester.ToString())
                .SingleOrDefault()
                .Elements("subject").Where(subj => subj.Attribute("name").Value == parent)
                .SingleOrDefault()
                .Elements("topic").Where(topic => topic.Attribute("name").Value == name)
                .SingleOrDefault().Attribute("isStudied").Value = "true";

            SaveDocument();
        }

        /// <summary>
        /// заполняем файл с личными данными 
        /// списком дисциплин
        /// </summary>
        public void WriteSemester()
        {
            var elements = XFile.Root.Elements("semester").Where(sem => sem.Attribute("number").Value == Cource.MyCource.Semester);

            if (elements.Count() == 0)
            {
                XFile.Root.Attribute("semester").Value = Cource.MyCource.Semester;
                XFile.Root.Attribute("courceNumber").Value = Cource.MyCource.CourceNumber;
                //создаем корневой элемент данного семестра
                using (XmlWriter writer = XFile.Root.CreateWriter())
                {
                    GenerateElement(writer, "semester", "");
                }

                //создаем 
                var semester = XFile.Root.Elements("semester").LastOrDefault();
                using (XmlWriter writer = semester.CreateWriter())
                {
                    GenerateAttribute(writer, "number", Cource.MyCource.Semester);
                }
                if (Cource.MyCource.Subjects != null)
                {
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
                            GenerateAttribute(innerWriter, "name", subj.Name);
                            GenerateAttribute(innerWriter, "hours", subj.Hours);
                            GenerateAttribute(innerWriter, "teacher", subj.Teacher);
                        }
                    }

                }

                SaveDocument();
            }
        }

        /// <summary>
        /// Сохранение списка тем в файл
        /// </summary>
        /// <param name="subj">сущность-дисциплина</param>
        public void FillTopicList(Subject subj)
        {
            foreach (var topic in subj.Topics)
            {
                //ищем дисциплину
                var subject = XFile.Root.Elements("semester")
                               .FirstOrDefault(elem => elem.Attribute("number").Value == Cource.MyCource.Semester)
                               .Elements("subject")
                               .SingleOrDefault(elem => elem.Attribute("name").Value == subj.Name);

                //запись в файл
                using (XmlWriter writer = subject.CreateWriter())
                {
                    this.GenerateElement(writer, "topic", "");
                }

                using (XmlWriter writer = subject.Elements("topic").LastOrDefault().CreateWriter())
                {
                    topic.Name = StorageProcessor.ReplaceCharacters(topic.Name, true);
                    this.GenerateAttribute(writer, "name", topic.Name);
                    this.GenerateAttribute(writer, "isStudied", "false");
                }

                //сохранение документа
                SaveDocument();
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
        private void GenerateAttribute(XmlWriter writer, string key, string value)
        {
            writer.WriteStartAttribute(key);
            writer.WriteString(value);
            writer.WriteEndAttribute();
        }
        
        #endregion
    }
}
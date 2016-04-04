using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecturer.Data.Entities
{
    /// <summary>
    /// Содержит информацию о курсе дисциплин,
    /// изучаемых в данном семестре
    /// </summary>
    public class Cource
    {
        private static Cource instance;

        private Cource() { }

        /// <summary>
        /// Курс дисциплин
        /// </summary>
        public static Cource MyCource
        {
            get
            {
                if (instance == null)
                {
                    instance = new Cource();
                }
                return instance;
            }
        }

        /// <summary>
        /// Возвращает путь к файлу с данными на сервере
        /// </summary>
        public string GetServerPath
        {
            get
            {
                return InstituteCode + @"/"
                       + SpecialityCode + @"/"
                       + Semester + @"/";
            }
        }

        /// <summary>
        /// Код института
        /// </summary>
        public string InstituteCode { get; set; }


        /// <summary>
        /// Корневая папка на локальном диске
        /// </summary>
        public string RootFolderPath { get; set; }

        /// <summary>
        /// курс
        /// </summary>
        public string CourceNumber { get; set; }

        /// <summary>
        /// Текущий семестр
        /// </summary>
        public string Semester { get; set; }
        
        /// <summary>
        /// Код групи
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Код специальности
        /// </summary>
        public string SpecialityCode { get; set; }

        /// <summary>
        /// Название специальности
        /// </summary>
        public string SpecialityName { get; set; }

        /// <summary>
        /// Выбранная дисциплина
        /// </summary>
        public Subject SelectedSubject { get; set; }

        /// <summary>
        /// Данные об изучаемых предметах
        /// </summary>
        public List<Subject> Subjects { get; set; } 
    }
    
}

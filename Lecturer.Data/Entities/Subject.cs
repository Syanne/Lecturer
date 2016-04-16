using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecturer.Data.Entities
{
    /// <summary>
    /// Данные о дисциплине
    /// </summary>
    public class Subject
    {
        /// <summary>
        /// название дисциплины
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// к-во часов
        /// </summary>
        public string Hours { get; set; }

        /// <summary>
        /// Выбранная тема
        /// </summary>
        public Topic SelectedTopic { get; set; }

        /// <summary>
        /// Преподаватель(ли)
        /// </summary>
        public string Teacher { get; set; }
       
        /// <summary>
        /// коллекция данных о темах, изучаемых в курсе
        /// </summary>
        public List<Topic> Topics { get; set; }
    }
    
}

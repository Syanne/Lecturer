using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecturer.Data.Entities
{
    /// <summary>
    /// Информация об изучаемой теме
    /// </summary>
    public class Topic
    {
        /// <summary>
        /// идентификатор темы
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// название темы
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Закладка
        /// </summary>
        //public string Bookmark { get; set; }

        /// <summary>
        /// Лекционный материал (html-страница)
        /// </summary>
        public string LectionUri { get; set; }

        /// <summary>
        /// флаг, указывающий, изучена ли дисциплина:
        /// false - не изучена, предложить тестирование
        /// true - изучена, предложить доп. материал
        /// </summary>
        public bool IsStudied { get; set; }

        /// <summary>
        /// Тест по пройденной теме
        /// </summary>
        public IEnumerable<QuizItem> Quiz { get; set; }
    }
}

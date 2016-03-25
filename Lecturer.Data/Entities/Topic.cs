using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;


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
        /// есть ли тест по пройденной теме
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// Цвет кружка (изучено - зеленый, нет - красный, лекция не доступна = серый)
        /// </summary>
        public System.Windows.Media.Brush CircleColor { get; set; }
    }
}

﻿using System;
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
        /// Текущий семестр
        /// </summary>
        public string CourceNumber { get; set; }        

        /// <summary>
        /// Текущий семестр
        /// </summary>
       // public string Semester { get; set; }

        /// <summary>
        /// Код групи
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Данные об изучаемых предметах
        /// </summary>
        public List<Subject> Subjects { get; set; } 
    }
    
}

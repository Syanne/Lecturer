using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecturer.Data.Entities
{
    public class Quiz
    {
        public int MinPoints { get; set; }
        public string TestName { get; set; }
        public List<QuizItem> Questions { get; set; }
    }

    public class QuizItem
    {
        public string Text { get; set; }
        public List<string> Answers { get; set; }
        public List<string> Values { get; set; }
        public bool IsOneTrue { get; set; }
    }
}

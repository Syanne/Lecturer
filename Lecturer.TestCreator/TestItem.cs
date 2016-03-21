using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecturer.TestCreator
{
    public class TestItem
    {
        public string Answer { get; set; }
        public bool IsTrue { get; set; }
        public int Tag { get; set; }
    }


    public class Questions
    {
        public List<TestItem> MyTest { get; set; }
        public string QuestionText { get; set; }
        public int TrueCount { get; set; }
    }
}

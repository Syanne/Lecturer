using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lecturer.Data.Entities
{
    public class UserData
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
        public int Tag { get; set; }
        public Visibility CanChangeValue { get; set; }
    }
}

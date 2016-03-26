using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecturer.Data.Entities
{
    public class University
    {
        public string Name { get; set; }
        public string FolderName { get; set; }
        public List<Speciality> Specialities { get; set; }
    }

    public class Speciality
    {
        public string Name { get; set; }
        public string FolderName { get; set; }
        public List<int> Cources { get; set; }
    }

}

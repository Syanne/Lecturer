using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lecturer.Data.Entities
{
    public class Institute
    {
        public string Name { get; set; }
        public string FolderName { get; set; }
        public List<Speciality> Specialities { get; set; }
    }

    public class Speciality
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string FolderName { get; set; }
        public List<int> Cources { get; set; }
    }

}

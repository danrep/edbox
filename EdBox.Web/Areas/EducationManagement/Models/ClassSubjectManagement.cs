using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdBox.Web.Areas.EducationManagement.Models
{
    public class ClassSubjectManagement
    {
        public ClassSubject ClassSubject { get; set; }
        public int? SubjectId { get; set; }
        public int? ClassId { get; set; }
        public string ClassName { get; set; }
        public string SubjectName { get; set; }
    }
}

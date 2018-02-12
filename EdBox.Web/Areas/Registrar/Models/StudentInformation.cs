using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdBox.Web.Areas.Registrar.Models
{
    public class StudentInformation
    {
        public string StudentId { get; set; }
        public string StudentSurname { get; set; }
        public string StudentOtherName { get; set; }
        public string StudentHomeAddress { get; set; }
        public string PgFullName { get; set; }
        public string PgEmail { get; set; }
        public int CurrentClassId { get; set; }
        public string PgPhone { get; set; }
        public string StudentImage { get; set; }
        public string BriefInformation { get; set; }
    }
}

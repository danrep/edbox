using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;

namespace EdBox.Core.FileSchema
{
    public class StudentAssessmentResult
    {
        public string StudentId { get; set; }
        public string MarksObtained { get; set; }
        public string MarksObtainable { get; set; }
    }

    public sealed class StudentAssessmentResultMap : CsvClassMap<StudentAssessmentResult>
    {
        public StudentAssessmentResultMap()
        {
            Map(m => m.StudentId).Index(0);
            Map(m => m.MarksObtained).Index(1);
            Map(m => m.MarksObtainable).Index(2);
        }
    }
}

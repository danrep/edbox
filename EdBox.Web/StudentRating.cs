//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EdBox.Web
{
    using System;
    using System.Collections.Generic;
    
    public partial class StudentRating
    {
        public long Id { get; set; }
        public string StudentId { get; set; }
        public int MarkedByCredentialId { get; set; }
        public int EducationalPeriod { get; set; }
        public int SubEducationalPeriod { get; set; }
        public int RatingType { get; set; }
        public int RatingValue { get; set; }
        public System.DateTime RatingDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}

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
    
    public partial class StudentSubjectMap
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public int SubjectId { get; set; }
        public System.DateTime DateAssigned { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    }
}

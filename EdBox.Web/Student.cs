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
    
    public partial class Student
    {
        public int Id { get; set; }
        public int AuuthCredentialId { get; set; }
        public string StudentId { get; set; }
        public string StudentSurname { get; set; }
        public string StudentOtherName { get; set; }
        public string StudentHomeAddress { get; set; }
        public int CurrentClassId { get; set; }
        public string PGFullName { get; set; }
        public string PGEmail { get; set; }
        public string PGPhone { get; set; }
        public string StudentImage { get; set; }
        public string BriefInformation { get; set; }
        public bool IsDeleted { get; set; }
    }
}
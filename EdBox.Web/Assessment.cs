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
    
    public partial class Assessment
    {
        public int Id { get; set; }
        public string AssessmentName { get; set; }
        public decimal AssmentPercentage { get; set; }
        public int AssessmentType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
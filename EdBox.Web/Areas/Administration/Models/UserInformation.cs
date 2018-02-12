using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EdBox.Web.Areas.Administration.Models
{
    public class UserInformation
    {
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int UserRole { get; set; }
    }
}
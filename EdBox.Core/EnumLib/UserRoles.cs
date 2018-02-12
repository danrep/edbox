using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdBox.Core.EnumLib
{
    public enum UserRoles : int
    {
        [EnumDisplayName(DisplayName = "Administration")]
        Administration = 1,
        [EnumDisplayName(DisplayName = "General Staff")]
        GeneralStaff,
        [EnumDisplayName(DisplayName = "Registrar")]
        Registrar,
        [EnumDisplayName(DisplayName = "Finance")]
        Finance,
        [EnumDisplayName(DisplayName = "Students")]
        Students = 10
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdBox.Core.EnumLib
{
    public enum PinState : int
    {
        [EnumDisplayName(DisplayName = "NoPinRequired")]
        NoPinRequired = 1,
        [EnumDisplayName(DisplayName = "PinRequiredNotFound")]
        PinRequiredNotFound,
        [EnumDisplayName(DisplayName = "PinRequiredFound")]
        PinRequiredFound
    }
}

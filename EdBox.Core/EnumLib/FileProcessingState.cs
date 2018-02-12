using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdBox.Core.EnumLib
{
    public enum FileProcessingState : int
    {
        [EnumDisplayName(DisplayName = "New")]
        New = 1,
        [EnumDisplayName(DisplayName = "Processing")]
        Processing,
        [EnumDisplayName(DisplayName = "Failed")]
        Failed,
        [EnumDisplayName(DisplayName = "Completed")]
        Completed
    }
}

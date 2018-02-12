
namespace EdBox.Core.EnumLib
{
    public enum AttendanceStates : int
    {
        [EnumDisplayName(DisplayName = "Present")]
        Present = 1,
        [EnumDisplayName(DisplayName = "Absent")]
        Absent,
        [EnumDisplayName(DisplayName = "Half Day")]
        Half
    }
}

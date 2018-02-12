
namespace EdBox.Core.EnumLib
{
    public enum FileProcessingMessageTypes : int
    {
        [EnumDisplayName(DisplayName = "Information")]
        Information = 1,
        [EnumDisplayName(DisplayName = "Warning")]
        Warning,
        [EnumDisplayName(DisplayName = "Error")]
        Error
    }
}

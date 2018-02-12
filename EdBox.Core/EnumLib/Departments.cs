
namespace EdBox.Core.EnumLib
{
    public enum Departments: int
    {
        [EnumDisplayName(DisplayName = "Nursery School: Normal")]
        Nursery = 1,
        [EnumDisplayName(DisplayName = "Primary School: Normal")]
        Primary,
        [EnumDisplayName(DisplayName = "Secondary School: Art")]
        SecondaryArt,
        [EnumDisplayName(DisplayName = "Secondary School: Commerce")]
        SecondaryCommerce,
        [EnumDisplayName(DisplayName = "Secondary School: Science")]
        SecondaryScience
    }
}

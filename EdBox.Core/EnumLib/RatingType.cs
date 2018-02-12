
namespace EdBox.Core.EnumLib
{
    public enum RatingType : int
    {
        [EnumDisplayName(DisplayName = "Punctuality")]
        Punctuality = 1,
        [EnumDisplayName(DisplayName = "Class Attendance")]
        ClassAttendance,
        [EnumDisplayName(DisplayName = "Carrying Out Assignment")]
        CarryingOutAssignment,
        [EnumDisplayName(DisplayName = "Neatness")]
        Neatness,
        [EnumDisplayName(DisplayName = "Politeness")]
        Politeness,
        [EnumDisplayName(DisplayName = "Relationship With Staff")]
        RelationshipWithStaff,
        [EnumDisplayName(DisplayName = "Relationship With Students")]
        RelationshipWithStudents,
        [EnumDisplayName(DisplayName = "Atttentiveness")]
        Atttentiveness,
        [EnumDisplayName(DisplayName = "Initiative")]
        Initiative,
        [EnumDisplayName(DisplayName = "Emotional Stability")]
        EmotionalStability,
        [EnumDisplayName(DisplayName = "Attitude To Study")]
        AttitudeToStudy,
        [EnumDisplayName(DisplayName = "Attitude To School")]
        AttitudeToSchool,
        [EnumDisplayName(DisplayName = "Attitude To Property")]
        AttitudeToProperty,
        [EnumDisplayName(DisplayName = "Clubs and Societies")]
        ClubsAndSocieties,
        [EnumDisplayName(DisplayName = "Games and Sports")]
        GamesAndSports,
        [EnumDisplayName(DisplayName = "Handwriting")]
        Handwriting,
        [EnumDisplayName(DisplayName = "Skills and Talents")]
        SkillsAndTalents
    }
}

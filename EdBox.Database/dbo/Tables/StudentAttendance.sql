CREATE TABLE [dbo].[StudentAttendance] (
    [Id]                     INT           IDENTITY (1, 1) NOT NULL,
    [StudentId]              VARCHAR (100) NOT NULL,
    [EducationalPeriodId]    INT           NOT NULL,
    [SubEducationalPeriodId] INT           NOT NULL,
    [AttendanceDate]         DATETIME      NOT NULL,
    [AttendanceStatus]       INT           NOT NULL,
    [ClassId]                INT           NOT NULL,
    [MarkedByCredentialId]   INT           NOT NULL,
    [IsDeleted]              BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


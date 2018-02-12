CREATE TABLE [dbo].[StudentSubjectMap] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [StudentId]    VARCHAR (100) NOT NULL,
    [SubjectId]    INT           NOT NULL,
    [DateAssigned] DATETIME      NOT NULL,
    [IsDeleted]    BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


CREATE TABLE [dbo].[StudentCommentFromTeacher] (
    [Id]                   BIGINT        IDENTITY (1, 1) NOT NULL,
    [StudentId]            VARCHAR (100) NOT NULL,
    [MarkedByCredentialId] INT           NOT NULL,
    [EducationalPeriod]    INT           NOT NULL,
    [SubEducationalPeriod] INT           NOT NULL,
    [CommentData]          VARCHAR (MAX) NOT NULL,
    [IsDeleted]            BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


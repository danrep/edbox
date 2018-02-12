CREATE TABLE [dbo].[SubjectMap] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [CredentialId] INT      NOT NULL,
    [SubjectId]    INT      NOT NULL,
    [IsDeleted]    BIT      NOT NULL,
    [DateAssigned] DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


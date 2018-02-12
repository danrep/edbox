CREATE TABLE [dbo].[ClassSubject] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [SessionId]    INT      NOT NULL,
    [SubSessionId] INT      NOT NULL,
    [SubjectId]    INT      NOT NULL,
    [ClassId]      INT      NOT NULL,
    [CredentialId] INT      NOT NULL,
    [DateCreated]  DATETIME NOT NULL,
    [IsDeleted]    BIT      NOT NULL,
    [IsActivated]  BIT      DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);




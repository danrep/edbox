CREATE TABLE [dbo].[ClassMap] (
    [Id]           INT      IDENTITY (1, 1) NOT NULL,
    [CredentialId] INT      NOT NULL,
    [ClassId]      INT      NOT NULL,
    [IsDeleted]    BIT      NOT NULL,
    [DateAssigned] DATETIME NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


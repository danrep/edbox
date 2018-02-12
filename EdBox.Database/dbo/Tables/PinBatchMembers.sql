CREATE TABLE [dbo].[PinBatchMembers] (
    [Id]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [BatchId]      INT           NOT NULL,
    [PinData]      VARCHAR (100) NOT NULL,
    [IsUsed]       BIT           NOT NULL,
    [DateUsed]     DATETIME      NOT NULL,
    [StudentId]    VARCHAR (100) NULL,
    [CredentialId] INT           NOT NULL,
    [IsDeleted]    BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


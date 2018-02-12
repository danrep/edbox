CREATE TABLE [dbo].[Transaction] (
    [Id]                       INT             IDENTITY (1, 1) NOT NULL,
    [TransactionDate]          DATETIME        NOT NULL,
    [CredentialId]             VARCHAR (100)   NOT NULL,
    [Amount]                   DECIMAL (18, 5) NOT NULL,
    [RequestJson]              VARCHAR (MAX)   NULL,
    [ResponseJson]             VARCHAR (MAX)   NULL,
    [SettlementTransactionRef] VARCHAR (100)   NULL,
    [TransactionStatus]        INT             NOT NULL,
    [IsDeleted]                BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


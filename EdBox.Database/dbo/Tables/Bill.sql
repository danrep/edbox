CREATE TABLE [dbo].[Bill] (
    [Id]                       INT           IDENTITY (1, 1) NOT NULL,
    [BillDate]                 DATETIME      NOT NULL,
    [StudentId]                VARCHAR (100) NOT NULL,
    [EducationalPeriodId]      INT           NOT NULL,
    [SubEducationalPeriod]     INT           NOT NULL,
    [BillItemId]               INT           NOT NULL,
    [BillStatusId]             INT           NOT NULL,
    [SettlementTransactionRef] VARCHAR (100) NULL,
    [IsDeleted]                BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


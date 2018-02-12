CREATE TABLE [dbo].[PinBatch] (
    [Id]                   INT      IDENTITY (1, 1) NOT NULL,
    [BatchDate]            DATETIME NOT NULL,
    [BatchSpace]           INT      NOT NULL,
    [EducationalPeriod]    INT      NOT NULL,
    [SubEducationalPeriod] INT      NOT NULL,
    [IsDeleted]            BIT      NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


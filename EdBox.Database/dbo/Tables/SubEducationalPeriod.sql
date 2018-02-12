CREATE TABLE [dbo].[SubEducationalPeriod] (
    [Id]                  INT           IDENTITY (1, 1) NOT NULL,
    [SubPeriodName]       VARCHAR (100) NOT NULL,
    [SubPeriodOrder]      INT           NOT NULL,
    [EducationalPeriodId] INT           NOT NULL,
    [PeriodStart]         DATETIME      NOT NULL,
    [PeriodEnd]           DATETIME      NOT NULL,
    [IsActive]            BIT           NOT NULL,
    [IsDeleted]           BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


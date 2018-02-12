CREATE TABLE [dbo].[EducationalPeriod] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [PeriodName]  VARCHAR (100) NOT NULL,
    [PeriodStart] DATETIME      NOT NULL,
    [PeriodEnd]   DATETIME      NOT NULL,
    [IsActive]    BIT           NOT NULL,
    [IsDeleted]   BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


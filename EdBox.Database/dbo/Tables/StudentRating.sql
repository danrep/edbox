CREATE TABLE [dbo].[StudentRating] (
    [Id]                   BIGINT        IDENTITY (1, 1) NOT NULL,
    [StudentId]            VARCHAR (100) NOT NULL,
    [MarkedByCredentialId] INT           NOT NULL,
    [EducationalPeriod]    INT           NOT NULL,
    [SubEducationalPeriod] INT           NOT NULL,
    [RatingType]           INT           NOT NULL,
    [RatingValue]          INT           NOT NULL,
    [RatingDate]           DATETIME      NOT NULL,
    [IsDeleted]            BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


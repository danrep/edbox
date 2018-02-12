CREATE TABLE [dbo].[Assessment] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [AssessmentName]    VARCHAR (100)   NOT NULL,
    [AssmentPercentage] DECIMAL (18, 5) NOT NULL,
    [AssessmentType]    INT             NOT NULL,
    [IsDeleted]         BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


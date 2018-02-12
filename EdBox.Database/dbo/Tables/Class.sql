CREATE TABLE [dbo].[Class] (
    [Id]          INT          IDENTITY (1, 1) NOT NULL,
    [ClassName]   VARCHAR (20) NOT NULL,
    [DateCreated] DATETIME     NOT NULL,
    [IsDeleted]   BIT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


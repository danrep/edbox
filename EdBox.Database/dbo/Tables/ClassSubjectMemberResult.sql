CREATE TABLE [dbo].[ClassSubjectMemberResult] (
    [Id]                   INT           IDENTITY (1, 1) NOT NULL,
    [ClassSubjectId]       INT           NOT NULL,
    [MemberResultName]     VARCHAR (100) NOT NULL,
    [TotalContributedMark] INT           NOT NULL,
    [MemberResultType]     INT           NOT NULL,
    [DateConfigured]       DATETIME      NOT NULL,
    [IsDeleted]            BIT           NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


CREATE TABLE [dbo].[ResultRaw] (
    [Id]            BIGINT          IDENTITY (1, 1) NOT NULL,
    [StudentId]     VARCHAR (100)   NOT NULL,
    [CredentialId]  INT             NOT NULL,
    [SessionId]     INT             NOT NULL,
    [SubSessionId]  INT             NOT NULL,
    [ClassId]       INT             NOT NULL,
    [SubjectId]     INT             NOT NULL,
    [AssessmentId]  INT             NOT NULL,
    [FileId]        INT             NOT NULL,
    [DateUploaded]  DATETIME        NOT NULL,
    [ScoreObtained] DECIMAL (18, 6) NOT NULL,
    [ScoreTotal]    DECIMAL (18, 6) NOT NULL,
    [IsDeleted]     BIT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);






CREATE TABLE [dbo].[UploadHistory] (
    [Id]                INT            IDENTITY (1, 1) NOT NULL,
    [CredentialId]      INT            NOT NULL,
    [SubjectId]         INT            NOT NULL,
    [ClassId]           INT            NOT NULL,
    [AssessmentId]      INT            NOT NULL,
    [RawFileName]       VARCHAR (1000) NOT NULL,
    [SystemFileName]    VARCHAR (1000) NOT NULL,
    [OperationMessages] VARCHAR (MAX)  NOT NULL,
    [FileSize]          BIGINT         NOT NULL,
    [ProcessingStatus]  INT            NOT NULL,
    [DateUploaded]      DATETIME       NOT NULL,
    [DateProcessStart]  DATETIME       NOT NULL,
    [DateProcessEnd]    DATETIME       NOT NULL,
    [IsDeleted]         BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);






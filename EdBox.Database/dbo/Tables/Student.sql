CREATE TABLE [dbo].[Student] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [AuuthCredentialId]  INT            NOT NULL,
    [StudentId]          VARCHAR (100)  NULL,
    [StudentSurname]     VARCHAR (100)  NULL,
    [StudentOtherName]   VARCHAR (100)  NULL,
    [StudentHomeAddress] VARCHAR (100)  NULL,
    [CurrentClassId]     INT            DEFAULT ((0)) NOT NULL,
    [PGFullName]         VARCHAR (100)  NULL,
    [PGEmail]            VARCHAR (100)  NULL,
    [PGPhone]            VARCHAR (100)  NULL,
    [StudentImage]       VARCHAR (MAX)  NULL,
    [BriefInformation]   VARCHAR (1000) NULL,
    [IsDeleted]          BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);




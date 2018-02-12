CREATE TABLE [dbo].[Credential] (
    [Id]             INT           IDENTITY (1, 1) NOT NULL,
    [Username]       VARCHAR (100) NOT NULL,
    [PasswordData]   VARCHAR (MAX) NOT NULL,
    [PasswordSalt]   VARCHAR (100) NOT NULL,
    [FirstName]      VARCHAR (100) NOT NULL,
    [LastName]       VARCHAR (100) NOT NULL,
    [UserState]      INT           NOT NULL,
    [DateRegistered] DATETIME      DEFAULT (getdate()) NOT NULL,
    [IsDeleted]      BIT           DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


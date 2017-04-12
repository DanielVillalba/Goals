CREATE TABLE [dbo].[Resource] (
    [ResourceId]        INT            NOT NULL,
    [Name]              NVARCHAR (250) NULL,
    [Email]             NVARCHAR (100) NULL,
    [RolId]             INT            NOT NULL,
    [DomainName]        NVARCHAR (50)  NOT NULL,
    [LastLogin]         DATETIME       NOT NULL,
    [LocationId]        INT            NULL,
    [ActiveDirectoryId] INT            NULL,
    [IsEnable]          INT            NULL,
    PRIMARY KEY CLUSTERED ([ResourceId] ASC),
    FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Location] ([ID])
);


CREATE TABLE [dbo].[Sources] (
    [SourceId] INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (100) NULL,
    CONSTRAINT [PK_Sources] PRIMARY KEY CLUSTERED ([SourceId] ASC)
);


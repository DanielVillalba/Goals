CREATE TABLE [dbo].[Technologies] (
    [TechnologyId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]         VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Technologies] PRIMARY KEY CLUSTERED ([TechnologyId] ASC)
);


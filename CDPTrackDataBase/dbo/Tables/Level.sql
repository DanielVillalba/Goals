CREATE TABLE [dbo].[Level] (
    [LevelId] INT           IDENTITY (1, 1) NOT NULL,
    [Name]    VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Level] PRIMARY KEY CLUSTERED ([LevelId] ASC)
);


CREATE TABLE [dbo].[Project] (
    [ProjectId] INT          IDENTITY (1, 1) NOT NULL,
    [Project]   VARCHAR (80) NULL,
    CONSTRAINT [PK_Project] PRIMARY KEY CLUSTERED ([ProjectId] ASC)
);


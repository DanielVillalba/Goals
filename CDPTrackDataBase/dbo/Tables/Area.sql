CREATE TABLE [dbo].[Area] (
    [AreaId]            INT            NOT NULL,
    [Name]              VARCHAR (2000) NOT NULL,
    [ImageCareerPath]   VARCHAR (1000) NULL,
    [ImageSkillCompass] VARCHAR (1000) NULL,
    CONSTRAINT [PK_Area] PRIMARY KEY CLUSTERED ([AreaId] ASC)
);


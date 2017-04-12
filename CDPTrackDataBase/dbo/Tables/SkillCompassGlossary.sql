CREATE TABLE [dbo].[SkillCompassGlossary] (
    [SkillCompassGlossaryId] INT           IDENTITY (1, 1) NOT NULL,
    [AreaId]                 INT           NOT NULL,
    [Name]                   VARCHAR (100) NULL,
    [Description]            VARCHAR (MAX) NULL,
    CONSTRAINT [PK_SkillCompassGlossary] PRIMARY KEY CLUSTERED ([SkillCompassGlossaryId] ASC),
    CONSTRAINT [FK_SkillCompassGlossary_Area] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Area] ([AreaId])
);


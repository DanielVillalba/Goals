CREATE TABLE [dbo].[PersonalDevelopment] (
    [PersonalDevelopmentId] INT           IDENTITY (1, 1) NOT NULL,
    [Skill]                 VARCHAR (255) NULL,
    [DefinitionOfSuccess]   VARCHAR (255) NULL,
    [Outcome]               VARCHAR (255) NULL,
    [QuarterlyPrioritiesId] INT           NULL,
    [Comment]               VARCHAR (255) NULL,
    CONSTRAINT [PK_PersonalDevelopment] PRIMARY KEY CLUSTERED ([PersonalDevelopmentId] ASC),
    CONSTRAINT [FK_PersonalDevelopment_QuarterlyPriorities] FOREIGN KEY ([QuarterlyPrioritiesId]) REFERENCES [dbo].[QuarterlyPriorities] ([QuarterlyPrioritiesId])
);






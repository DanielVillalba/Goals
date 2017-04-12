CREATE TABLE [dbo].[Suggestions] (
    [SuggestionId] INT            IDENTITY (1, 1) NOT NULL,
    [PositionId]   INT            NOT NULL,
    [TechnologyId] INT            NOT NULL,
    [LevelId]      INT            NOT NULL,
    [Topic]        VARCHAR (1000) NOT NULL,
    [Source]       VARCHAR (1000) NOT NULL,
    [AreaId]       INT            NULL,
    CONSTRAINT [PK_Suggestions] PRIMARY KEY CLUSTERED ([SuggestionId] ASC),
    CONSTRAINT [FK_Suggestions_Area] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Area] ([AreaId]),
    CONSTRAINT [FK_Suggestions_Level] FOREIGN KEY ([LevelId]) REFERENCES [dbo].[Level] ([LevelId]),
    CONSTRAINT [FK_Suggestions_Position] FOREIGN KEY ([PositionId]) REFERENCES [dbo].[Position] ([PositionId]),
    CONSTRAINT [FK_Suggestions_Technologies] FOREIGN KEY ([TechnologyId]) REFERENCES [dbo].[Technologies] ([TechnologyId])
);


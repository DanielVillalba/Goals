CREATE TABLE [dbo].[Position] (
    [PositionId]   INT            IDENTITY (1, 1) NOT NULL,
    [PositionName] VARCHAR (200)  NOT NULL,
    [AreaId]       INT            NOT NULL,
    [Description]  VARCHAR (1000) NULL,
    CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED ([PositionId] ASC),
    CONSTRAINT [FK_Position_Area] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Area] ([AreaId])
);


CREATE TABLE [dbo].[PositionsHierarchy] (
    [PositionHierarchyId] INT IDENTITY (1, 1) NOT NULL,
    [PositionId]          INT NOT NULL,
    [NextPosition]        INT NOT NULL,
    CONSTRAINT [PK_PositionsHierarchy] PRIMARY KEY CLUSTERED ([PositionHierarchyId] ASC),
    CONSTRAINT [FK_PositionsHierarchy_Position] FOREIGN KEY ([PositionId]) REFERENCES [dbo].[Position] ([PositionId]),
    CONSTRAINT [FK_PositionsHierarchy_Position1] FOREIGN KEY ([NextPosition]) REFERENCES [dbo].[Position] ([PositionId])
);


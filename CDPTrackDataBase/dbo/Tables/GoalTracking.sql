CREATE TABLE [dbo].[GoalTracking] (
    [ResourceId]         INT             NOT NULL,
    [Goal]               NVARCHAR (1000) NOT NULL,
    [Progress]           INT             NOT NULL,
    [GoalId]             INT             IDENTITY (1, 1) NOT NULL,
    [FinishDate]         DATETIME        NULL,
    [VerifiedByManager]  BIT             NOT NULL,
    [LastUpdate]         DATETIME        NULL,
    [ObjectiveId]        INT             NULL,
    [TrainingCategoryId] INT             NULL,
    [TDU]                INT             NULL,
    [SourceId]           INT             NULL,
    PRIMARY KEY CLUSTERED ([GoalId] ASC),
    FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId]),
    CONSTRAINT [FK_GoalTracking_ProgressEnum] FOREIGN KEY ([Progress]) REFERENCES [dbo].[ProgressEnum] ([Id]),
    CONSTRAINT [FK_GoalTracking_SourceId] FOREIGN KEY ([SourceId]) REFERENCES [dbo].[Sources] ([SourceId]) ON DELETE CASCADE ON UPDATE CASCADE NOT FOR REPLICATION,
    CONSTRAINT [FK_GoalTracking_TrainingCategory] FOREIGN KEY ([TrainingCategoryId]) REFERENCES [dbo].[TrainingCategory] ([TrainingCategoryId]) ON UPDATE CASCADE
);


GO
ALTER TABLE [dbo].[GoalTracking] NOCHECK CONSTRAINT [FK_GoalTracking_SourceId];


GO
ALTER TABLE [dbo].[GoalTracking] NOCHECK CONSTRAINT [FK_GoalTracking_TrainingCategory];


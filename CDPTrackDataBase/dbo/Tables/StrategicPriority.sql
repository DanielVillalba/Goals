CREATE TABLE [dbo].[StrategicPriority] (
    [StrategicPriorityId]   INT            NOT NULL,
    [Action]                VARCHAR (255)  NULL,
    [Weight]                DECIMAL (5, 2) NULL,
    [DefinitionOfDone]      VARCHAR (255)  NULL,
    [DueDate]               DATE           NULL,
    [Result]                VARCHAR (255)  NULL,
    [Comment]               VARCHAR (255)  NULL,
    [QuarterlyPrioritiesId] INT            NOT NULL,
    [AnnualPrioritiesID]    INT            CONSTRAINT [DF_StrategicPriority_AnnualPrioritiesID] DEFAULT ((0)) NOT NULL,
    [RankID]                INT            NOT NULL,
    CONSTRAINT [PK__Strategi__E5CE356759F07B3F] PRIMARY KEY CLUSTERED ([StrategicPriorityId] ASC),
    CONSTRAINT [FK_StrategicPriority_AnnualPriorities] FOREIGN KEY ([AnnualPrioritiesID]) REFERENCES [dbo].[AnnualPriorities] ([AnnualPrioritiesId]),
    CONSTRAINT [FK_StrategicPriority_QuarterlyPriorities] FOREIGN KEY ([QuarterlyPrioritiesId]) REFERENCES [dbo].[QuarterlyPriorities] ([QuarterlyPrioritiesId]),
    CONSTRAINT [FK_StrategicPriority_RankCatalog] FOREIGN KEY ([RankID]) REFERENCES [dbo].[RankCatalog] ([RankID])
);





CREATE TABLE [dbo].[QuarterlyActions] (
    [QuarterlyActionId]     INT           IDENTITY (1, 1) NOT NULL,
    [Action]                VARCHAR (255) NULL,
    [DueDate]               DATE          NULL,
    [KeyIniciative]         INT           NULL,
    [Outcome]               VARCHAR (255) NULL,
    [QuarterlyPrioritiesId] INT           NULL,
    CONSTRAINT [PK_QuarterlyActions] PRIMARY KEY CLUSTERED ([QuarterlyActionId] ASC),
    CONSTRAINT [FK_QuarterlyActions_QuarterlyPriorities] FOREIGN KEY ([QuarterlyPrioritiesId]) REFERENCES [dbo].[QuarterlyPriorities] ([QuarterlyPrioritiesId])
);


CREATE TABLE [dbo].[ValuesInfusion] (
    [ValuesInfusionId]      INT           IDENTITY (1, 1) NOT NULL,
    [Value]                 VARCHAR (255) NULL,
    [DefinitionOfDone]      VARCHAR (255) NULL,
    [IsDone]                BIT           NULL,
    [Result]                VARCHAR (255) NULL,
    [QuarterlyPrioritiesId] INT           NULL,
    CONSTRAINT [PK_ValuesInfusion] PRIMARY KEY CLUSTERED ([ValuesInfusionId] ASC),
    CONSTRAINT [FK_ValuesInfusion_QuarterlyPriorities] FOREIGN KEY ([QuarterlyPrioritiesId]) REFERENCES [dbo].[QuarterlyPriorities] ([QuarterlyPrioritiesId])
);




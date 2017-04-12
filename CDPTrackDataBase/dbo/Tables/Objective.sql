CREATE TABLE [dbo].[Objective] (
    [ObjectiveId] INT             IDENTITY (1, 1) NOT NULL,
    [Objective]   NVARCHAR (1000) NULL,
    [CategoryId]  INT             NOT NULL,
    [ResourceId]  INT             NOT NULL,
    [Year]        INT             NULL,
    [Quarter]     INT             NULL,
    [Progress]    INT             NULL,
    [Duplicated]  BIT             NULL,
    CONSTRAINT [PK_Objective] PRIMARY KEY CLUSTERED ([ObjectiveId] ASC),
    CONSTRAINT [FK_Objective_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category] ([CategoryId]),
    CONSTRAINT [FK_Objective_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId]),
    CONSTRAINT [fk_Progress] FOREIGN KEY ([Progress]) REFERENCES [dbo].[ProgressEnum] ([Id])
);


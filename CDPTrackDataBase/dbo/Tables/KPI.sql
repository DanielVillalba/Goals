CREATE TABLE [dbo].[KPI] (
    [KpiID]                INT            IDENTITY (1, 1) NOT NULL,
    [QuaterlyPrioritiesID] INT            NOT NULL,
    [Name]                 VARCHAR (255)  NULL,
    [Weight]               DECIMAL (5, 2) NULL,
    [Result]               VARCHAR (50)   NULL,
    [Comment]              VARCHAR (150)  NULL,
    [RankID]               INT            NOT NULL,
    CONSTRAINT [PK_KPI] PRIMARY KEY CLUSTERED ([KpiID] ASC),
    CONSTRAINT [FK_KPI_QuarterlyPriorities] FOREIGN KEY ([QuaterlyPrioritiesID]) REFERENCES [dbo].[QuarterlyPriorities] ([QuarterlyPrioritiesId]),
    CONSTRAINT [FK_KPI_RankCatalog] FOREIGN KEY ([RankID]) REFERENCES [dbo].[RankCatalog] ([RankID])
);






GO



GO



GO



GO



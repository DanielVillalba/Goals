CREATE TABLE [dbo].[Threshold] (
    [ThresholdId] INT             IDENTITY (1, 1) NOT NULL,
    [KpiID]       INT             NOT NULL,
    [RankID]      INT             NOT NULL,
    [Value]       NUMERIC (18, 2) NOT NULL,
    CONSTRAINT [PK__Threshold__55855B85C9008A75] PRIMARY KEY CLUSTERED ([ThresholdId] ASC),
    CONSTRAINT [FK_Threshold_RankCatalog] FOREIGN KEY ([RankID]) REFERENCES [dbo].[RankCatalog] ([RankID])
);




CREATE TABLE [dbo].[RankCatalog] (
    [RankID]      INT          IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    [Score]       SMALLINT     CONSTRAINT [DF_RankCatalog_Score] DEFAULT ((0)) NOT NULL,
    [RankTypeID]  TINYINT      NOT NULL,
    [GroupId]     INT          NOT NULL,
    [IsActive]    BIT          CONSTRAINT [DF_RankCatalog_IsActive] DEFAULT ((1)) NOT NULL,
    [IsVisible]   BIT          CONSTRAINT [DF_RankCatalog_IsVisible] DEFAULT ((1)) NOT NULL,
    [LevelRankID] TINYINT      NOT NULL,
    CONSTRAINT [PK__RankCata__B37AFB963963B7B3] PRIMARY KEY CLUSTERED ([RankID] ASC),
    CONSTRAINT [FK_RankCatalog_Groups] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([GroupId]),
    CONSTRAINT [FK_RankCatalog_LevelRank] FOREIGN KEY ([LevelRankID]) REFERENCES [dbo].[LevelRank] ([LevelRankID]),
    CONSTRAINT [FK_RankCatalog_TypeCatalog] FOREIGN KEY ([RankTypeID]) REFERENCES [dbo].[RankTypeCatalog] ([RankTypeID])
);



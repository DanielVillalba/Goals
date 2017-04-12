CREATE TABLE [dbo].[LevelRank] (
    [LevelRankID] TINYINT      IDENTITY (1, 1) NOT NULL,
    [Name]        VARCHAR (20) NOT NULL,
    CONSTRAINT [PK_LevelRank] PRIMARY KEY CLUSTERED ([LevelRankID] ASC)
);


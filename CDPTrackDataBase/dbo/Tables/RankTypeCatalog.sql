CREATE TABLE [dbo].[RankTypeCatalog] (
    [RankTypeID] TINYINT       IDENTITY (1, 1) NOT NULL,
    [TypeName]   VARCHAR (50)  NOT NULL,
    [Comments]   VARCHAR (255) NULL,
    CONSTRAINT [PK__RankType__516F039531CF10D1] PRIMARY KEY CLUSTERED ([RankTypeID] ASC)
);



CREATE TABLE [dbo].[TrainingCategory] (
    [TrainingCategoryId] INT             NOT NULL,
    [Name]               NVARCHAR (100)  NULL,
    [Description]        NVARCHAR (1000) NULL,
    [TDU]                INT             NULL,
    [MaxTDU]             INT             NULL,
    CONSTRAINT [PK_TrainingCategory] PRIMARY KEY CLUSTERED ([TrainingCategoryId] ASC),
    CONSTRAINT [FK_TrainingCategory_TrainingCategory] FOREIGN KEY ([TrainingCategoryId]) REFERENCES [dbo].[TrainingCategory] ([TrainingCategoryId])
);


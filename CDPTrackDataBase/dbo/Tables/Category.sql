CREATE TABLE [dbo].[Category] (
    [CategoryId] INT           IDENTITY (1, 1) NOT NULL,
    [Category]   NVARCHAR (50) NULL,
    [Visibility] BIT           NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([CategoryId] ASC)
);


CREATE TABLE [dbo].[Term] (
    [TermId] INT          IDENTITY (1, 1) NOT NULL,
    [Name]   VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Term] PRIMARY KEY CLUSTERED ([TermId] ASC)
);


CREATE TABLE [dbo].[Location] (
    [ID]           INT          NOT NULL,
    [Name]         VARCHAR (50) NOT NULL,
    [abbreviation] VARCHAR (3)  NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    UNIQUE NONCLUSTERED ([Name] ASC)
);


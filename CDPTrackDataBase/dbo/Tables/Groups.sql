CREATE TABLE [dbo].[Groups] (
    [GroupId]   INT           IDENTITY (1, 1) NOT NULL,
    [GroupName] NVARCHAR (50) NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED ([GroupId] ASC)
);


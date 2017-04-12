CREATE TABLE [dbo].[Employee_Groups] (
    [ResourceId] INT          NOT NULL,
    [GroupId]    VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Employee_Groups] PRIMARY KEY CLUSTERED ([ResourceId] ASC, [GroupId] ASC)
);


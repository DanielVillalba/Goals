CREATE TABLE [dbo].[Quarter]
(
	[QuarterID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(30) NULL, 
    [StartDate] DATE NULL, 
    [EndDate] DATE NULL, 
    [Comment] VARCHAR(150) NULL
)

USE [CDPTrack]
GO

--CREATE TABLE TrainingCategory
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrainingCategory](
	[TrainingCategoryId] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Description] [nvarchar](1000) NULL,
	[TDU] [int] NULL,
	[MaxTDU] [int] NULL,
 CONSTRAINT [PK_TrainingCategory] PRIMARY KEY CLUSTERED 
(
	[TrainingCategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TrainingCategory]  WITH CHECK ADD  CONSTRAINT [FK_TrainingCategory_TrainingCategory] FOREIGN KEY([TrainingCategoryId])
REFERENCES [dbo].[TrainingCategory] ([TrainingCategoryId])
GO

ALTER TABLE [dbo].[TrainingCategory] CHECK CONSTRAINT [FK_TrainingCategory_TrainingCategory]
GO

--ADD TrainingCategoryId AND TDU TO TABLE GoalTracking
ALTER TABLE GoalTracking
ADD TrainingCategoryId int,
	TDU int
GO

ALTER TABLE [dbo].[GoalTracking]  WITH NOCHECK ADD  CONSTRAINT [FK_GoalTracking_TrainingCategory] FOREIGN KEY([TrainingCategoryId])
REFERENCES [dbo].[TrainingCategory] ([TrainingCategoryId])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[GoalTracking] NOCHECK CONSTRAINT [FK_GoalTracking_TrainingCategory]
GO

UPDATE GoalTracking SET TDU = 0;
GO

--ADD Duplicated FIELD TO TABLE Objective
ALTER TABLE Objective
ADD Duplicated bit
GO

UPDATE Objective SET Duplicated = 0
GO

--ADD Visibility FIELD TO TABLE Category
ALTER TABLE Category
ADD Visibility bit
GO

UPDATE Category SET Visibility = 0 WHERE Category = 'Organizational' OR Category = 'Unassign'
GO

UPDATE Category SET Visibility = 1 WHERE Category = 'Technical' OR Category = 'Soft Skills'
GO

--CREATE TABLE Sources
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Sources](
	[SourceId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
 CONSTRAINT [PK_Sources] PRIMARY KEY CLUSTERED 
(
	[SourceId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--ADD SourceId FIELD TO TABLE GoalTracking
ALTER TABLE GoalTracking
ADD SourceId int
GO

ALTER TABLE [dbo].[GoalTracking]  WITH NOCHECK ADD  CONSTRAINT [FK_GoalTracking_SourceId] FOREIGN KEY([SourceId])
REFERENCES [dbo].[Sources] ([SourceId])
ON UPDATE CASCADE
ON DELETE CASCADE
NOT FOR REPLICATION 
GO

ALTER TABLE [dbo].[GoalTracking] NOCHECK CONSTRAINT [FK_GoalTracking_SourceId]
GO

--FILL TABLES Sources AND TrainingCategory
INSERT INTO Sources VALUES ('SafariBooksOnline')
GO

INSERT INTO Sources VALUES ('Degreed')
GO

INSERT INTO Sources VALUES ('TiempoLibrary')
GO

INSERT INTO TrainingCategory VALUES
(1,
'Reading and recommending a book', 
'Write a detailed book recommendation for Tiempo Library or the Bulletin. Each item in this category equals 3 points. You may earn up to 9 points in this category per quarter.',
3,
9)
GO

INSERT INTO TrainingCategory VALUES
(2,
'Enroll in on-demand training', 
'Complete Soft skills training program, Webinar. Each item in this category equals 1 point. You may earn up to 16 points in this category per quarter.',
1,
16)
GO

INSERT INTO TrainingCategory VALUES
(3,
'Enroll in course or training',
'Tiempo official technical trainings. Coursera and other online courses with exercises and evaluations. Each item in this category equals 3 points. You may earn up to 9 points in this category per quarter.',
3,
9)
GO

INSERT INTO TrainingCategory VALUES
(4,
'Design and Facilitate Tiempo official in house training',
'Technical trainings, not process related. Each item in this category equals 3 points. You may earn up to 15 points in this category per quarter.',
3,
15)
GO

INSERT INTO TrainingCategory VALUES
(5,
'Technically coach a person',
'Have constant meetings with coachee, exercises. Each item in this category equals 5 points. You may earn up to 5 points in this category per quarter.',
5,
5)
GO

--UPDATE GoalsView
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[GoalsView]
AS
SELECT        TOP (100) PERCENT g.GoalId, dbo.Location.Name, er.ManagerId, e.Name AS Manager, e.ResourceId, e.Name AS Employee, g.Goal, g.FinishDate, 
                         CASE WHEN VerifiedByManager = 1 THEN 1 ELSE 0 END AS Verified, CASE WHEN (YEAR(g.FinishDate) <= YEAR(GETDATE())) AND (MONTH(g.FinishDate) 
                         >= MONTH(GETDATE())) THEN 1 ELSE 0 END AS MustCheck, g.Progress, g.TDU
FROM            dbo.GoalTracking AS g INNER JOIN
                         dbo.Employee AS er ON er.ResourceId = g.ResourceId INNER JOIN
                         dbo.Resource AS e ON er.ResourceId = e.ResourceId INNER JOIN
                         dbo.Resource AS m ON er.ManagerId = m.ResourceId INNER JOIN
                         dbo.Location ON e.LocationId = dbo.Location.ID
ORDER BY MANAGER, EMPLOYEE, g.FinishDate, MUSTCHECK

GO

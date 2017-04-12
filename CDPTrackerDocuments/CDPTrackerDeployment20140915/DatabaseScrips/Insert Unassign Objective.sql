USE [CDPTrack]
GO

INSERT [dbo].[Category] ([Category]) VALUES ('Unassign')

GO

CREATE TABLE #tableResourceId (ResourceId INT);
INSERT INTO #tableResourceId SELECT ResourceId FROM [dbo].[Resource];

INSERT INTO [dbo].[Objective] (Objective, CategoryId, Year, ResourceId)
SELECT 'Unassign Objectives', 4, 2014, ResourceId FROM #tableResourceId

GO

CREATE TABLE #tableObjectiveId (ObjectiveId INT, ResourceId INT, CategoryId INT);
INSERT INTO #tableObjectiveId SELECT ObjectiveId, ResourceId, CategoryId FROM [dbo].[Objective] WHERE CategoryId = 4

UPDATE [dbo].[GoalTracking]
SET ObjectiveId = B.ObjectiveId
FROM GoalTracking A INNER JOIN #tableObjectiveId B ON A.ResourceId = B.ResourceId

GO
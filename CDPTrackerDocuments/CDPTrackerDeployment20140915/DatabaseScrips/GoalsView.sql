USE [CDPTrack]
GO

/****** Object:  View [dbo].[GoalsView]    Script Date: 7/22/2014 11:30:06 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[GoalsView]
AS
SELECT     TOP (100) PERCENT g.GoalId, dbo.Location.Name, er.ManagerId, e.Name AS Manager, e.ResourceId, e.Name AS Employee, g.Goal, g.FinishDate, 
                      CASE WHEN VerifiedByManager = 1 THEN 1 ELSE 0 END AS Verified, CASE WHEN (YEAR(g.FinishDate) <= YEAR(GETDATE())) AND (MONTH(g.FinishDate) 
                      >= MONTH(GETDATE())) THEN 1 ELSE 0 END AS MustCheck, g.Progress
FROM         dbo.GoalTracking AS g INNER JOIN
                      dbo.Employee AS er ON er.ResourceId = g.ResourceId INNER JOIN
                      dbo.Resource AS e ON er.ResourceId = e.ResourceId INNER JOIN
                      dbo.Resource AS m ON er.ManagerId = m.ResourceId INNER JOIN
                      dbo.Location ON e.LocationId = dbo.Location.ID
ORDER BY MANAGER, EMPLOYEE, g.FinishDate, MUSTCHECK

GO


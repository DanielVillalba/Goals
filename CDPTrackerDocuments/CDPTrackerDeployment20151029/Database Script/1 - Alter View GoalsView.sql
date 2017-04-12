use CDPTrack;
go

go
ALTER VIEW GoalsView as
SELECT        TOP (100) PERCENT g.GoalId, m.ResourceId  as ManagerId, m.Name AS Manager, e.ResourceId, e.Name AS Employee, 
			  el.Name AS ManagerLocation , ml.Name AS EmployeeLocation, g.Goal, o.Objective, g.FinishDate, 
                         CASE WHEN VerifiedByManager = 1 THEN 1 ELSE 0 END AS Verified, CASE WHEN (YEAR(g.FinishDate) <= YEAR(GETDATE())) AND (MONTH(g.FinishDate) 
                         >= MONTH(GETDATE())) THEN 1 ELSE 0 END AS MustCheck, g.Progress, g.TDU, tc.Name AS TrainingCategory
FROM            dbo.GoalTracking AS g INNER JOIN
                         dbo.Employee AS er ON er.ResourceId = g.ResourceId INNER JOIN
                         dbo.Resource AS e ON er.ResourceId = e.ResourceId INNER JOIN
                         dbo.Resource AS m ON er.ManagerId = m.ResourceId INNER JOIN
                         dbo.Objective AS o ON g.ObjectiveId = o.ObjectiveId INNER JOIN
						 dbo.Location AS el on m.LocationId = el.ID INNER JOIN
						 dbo.Location as ml on e.LocationId = ml.ID INNER JOIN
                         dbo.TrainingCategory AS tc ON tc.TrainingCategoryId = g.TrainingCategoryId INNER JOIN
                         dbo.Location ON e.LocationId = dbo.Location.ID
ORDER BY MANAGER, EMPLOYEE, g.FinishDate, MUSTCHECK
go
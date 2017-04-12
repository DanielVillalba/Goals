CREATE VIEW GoalsView as
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
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 2, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'GoalsView';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane2', @value = N'    Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'GoalsView';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[30] 4[22] 2[30] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "g"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "er"
            Begin Extent = 
               Top = 6
               Left = 265
               Bottom = 135
               Right = 441
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "e"
            Begin Extent = 
               Top = 6
               Left = 479
               Bottom = 135
               Right = 649
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "m"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 267
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Location"
            Begin Extent = 
               Top = 138
               Left = 246
               Bottom = 233
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 11
         Width = 284
         Width = 1500
         Width = 1500
         Width = 21630
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 10560
     ', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'GoalsView';



CREATE VIEW [dbo].[GeneralTrainingProgramVisitsView]
AS
SELECT        dbo.GeneralTrainingProgram.Name, dbo.Resource.Name AS Resource, COUNT(*) AS Visits, dbo.GeneralTrainingProgramVisits.VisitDate, 
                         dbo.TrainingProgramCategory.Name AS Category
FROM            dbo.GeneralTrainingProgramVisits INNER JOIN
                         dbo.Resource ON dbo.GeneralTrainingProgramVisits.ResourceId = dbo.Resource.ResourceId INNER JOIN
                         dbo.GeneralTrainingProgram ON dbo.GeneralTrainingProgramVisits.IdGeneralTrainingProgram = dbo.GeneralTrainingProgram.IdGeneralTrainingProgram INNER JOIN
                         dbo.TrainingProgramCategory ON dbo.TrainingProgramCategory.IdTrainingProgramCategory = dbo.GeneralTrainingProgram.Category
GROUP BY dbo.GeneralTrainingProgram.Name, dbo.Resource.Name, dbo.GeneralTrainingProgramVisits.VisitDate, dbo.TrainingProgramCategory.Name
GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPaneCount', @value = 1, @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'GeneralTrainingProgramVisitsView';


GO
EXECUTE sp_addextendedproperty @name = N'MS_DiagramPane1', @value = N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
         Begin Table = "GeneralTrainingProgramVisits"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 267
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Resource"
            Begin Extent = 
               Top = 6
               Left = 305
               Bottom = 135
               Right = 485
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "GeneralTrainingProgram"
            Begin Extent = 
               Top = 6
               Left = 523
               Bottom = 135
               Right = 752
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
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
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
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 1080
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
', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'VIEW', @level1name = N'GeneralTrainingProgramVisitsView';


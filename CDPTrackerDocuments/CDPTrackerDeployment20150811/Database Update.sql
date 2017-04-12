USE [CDPTrack]
GO

-- Create dbo.CDPView 
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CDPView]
AS
SELECT        dbo.Position.PositionName, COUNT(*) AS CDP, dbo.Location.Name, dbo.Objective.Year, dbo.Objective.Quarter
FROM            dbo.Objective INNER JOIN
                         dbo.Location INNER JOIN
                         dbo.Employee INNER JOIN
                         dbo.Position ON dbo.Employee.CurrentPositionID = dbo.Position.PositionId INNER JOIN
                         dbo.Resource ON dbo.Employee.ResourceId = dbo.Resource.ResourceId ON dbo.Location.ID = dbo.Resource.LocationId ON 
                         dbo.Objective.ResourceId = dbo.Resource.ResourceId
GROUP BY dbo.Position.PositionName, dbo.Location.Name, dbo.Objective.Year, dbo.Objective.Quarter

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
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
         Begin Table = "Objective"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Location"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 118
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Employee"
            Begin Extent = 
               Top = 6
               Left = 454
               Bottom = 135
               Right = 641
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Position"
            Begin Extent = 
               Top = 120
               Left = 246
               Bottom = 249
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Resource"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 267
               Right = 218
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
    ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CDPView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     Table = 1170
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CDPView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'CDPView'
GO

-- Add Other to dbo.Position
INSERT INTO dbo.Position VALUES ('Other', 0, null)
GO

-- Add Other to dbo.Technologies
INSERT INTO dbo.Technologies VALUES ('Other')
GO

-- Add Ohter to dbo.Level
INSERT INTO dbo.Level VALUES ('Other')
GO

-- Create dbo.GeneralTrainingProgramVisits
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GeneralTrainingProgramVisits](
	[IdVisit] [int] IDENTITY(1,1) NOT NULL,
	[IdGeneralTrainingProgram] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[VisitDate] [date] NOT NULL,
 CONSTRAINT [PK_GeneralTrainingProgramVisits] PRIMARY KEY CLUSTERED 
(
	[IdVisit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[GeneralTrainingProgramVisits]  WITH CHECK ADD  CONSTRAINT [FK_GeneralTrainingProgramVisits_GeneralTrainingProgram] FOREIGN KEY([IdGeneralTrainingProgram])
REFERENCES [dbo].[GeneralTrainingProgram] ([IdGeneralTrainingProgram])
GO

ALTER TABLE [dbo].[GeneralTrainingProgramVisits] CHECK CONSTRAINT [FK_GeneralTrainingProgramVisits_GeneralTrainingProgram]
GO

ALTER TABLE [dbo].[GeneralTrainingProgramVisits]  WITH CHECK ADD  CONSTRAINT [FK_GeneralTrainingProgramVisits_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO

ALTER TABLE [dbo].[GeneralTrainingProgramVisits] CHECK CONSTRAINT [FK_GeneralTrainingProgramVisits_Resource]
GO

-- Create dbo.TrainingProgramVisits
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrainingProgramVisits](
	[IdVisit] [int] IDENTITY(1,1) NOT NULL,
	[IdTrainingProgram] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[VisitDate] [date] NOT NULL,
 CONSTRAINT [PK_TrainingProgramVisits] PRIMARY KEY CLUSTERED 
(
	[IdVisit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TrainingProgramVisits]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramVisits_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO

ALTER TABLE [dbo].[TrainingProgramVisits] CHECK CONSTRAINT [FK_TrainingProgramVisits_Resource]
GO

ALTER TABLE [dbo].[TrainingProgramVisits]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramVisits_TrainingProgram] FOREIGN KEY([IdTrainingProgram])
REFERENCES [dbo].[TrainingProgram] ([IdTrainingProgram])
GO

ALTER TABLE [dbo].[TrainingProgramVisits] CHECK CONSTRAINT [FK_TrainingProgramVisits_TrainingProgram]
GO

-- Create dbo.TrainingProgramOnDemandVisits
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrainingProgramOnDemandVisits](
	[IdVisit] [int] IDENTITY(1,1) NOT NULL,
	[IdTrainingProgramOnDemand] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[VisitDate] [date] NOT NULL,
 CONSTRAINT [PK_TrainingProgramOnDemandVisits] PRIMARY KEY CLUSTERED 
(
	[IdVisit] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TrainingProgramOnDemandVisits]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramOnDemandVisits_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO

ALTER TABLE [dbo].[TrainingProgramOnDemandVisits] CHECK CONSTRAINT [FK_TrainingProgramOnDemandVisits_Resource]
GO

ALTER TABLE [dbo].[TrainingProgramOnDemandVisits]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramOnDemandVisits_TrainingProgramOnDemand] FOREIGN KEY([IdTrainingProgramOnDemand])
REFERENCES [dbo].[TrainingProgramOnDemand] ([IdTrainingProgramOnDemand])
GO

ALTER TABLE [dbo].[TrainingProgramOnDemandVisits] CHECK CONSTRAINT [FK_TrainingProgramOnDemandVisits_TrainingProgramOnDemand]
GO

-- Create dbo.GeneralTrainingProgramVisitsView
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[GeneralTrainingProgramVisitsView]
AS
SELECT        dbo.GeneralTrainingProgram.Name, dbo.Resource.Name AS Resource, COUNT(*) AS Visits, dbo.GeneralTrainingProgramVisits.VisitDate
FROM            dbo.GeneralTrainingProgramVisits INNER JOIN
                         dbo.Resource ON dbo.GeneralTrainingProgramVisits.ResourceId = dbo.Resource.ResourceId INNER JOIN
                         dbo.GeneralTrainingProgram ON dbo.GeneralTrainingProgramVisits.IdGeneralTrainingProgram = dbo.GeneralTrainingProgram.IdGeneralTrainingProgram
GROUP BY dbo.GeneralTrainingProgram.Name, dbo.Resource.Name, dbo.GeneralTrainingProgramVisits.VisitDate

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GeneralTrainingProgramVisitsView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'GeneralTrainingProgramVisitsView'
GO

-- Create dbo.TrainingProgramVisitsView
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TrainingProgramVisitsView]
AS
SELECT        dbo.TrainingProgram.Name, dbo.Resource.Name AS Resource, COUNT(*) AS Visits, dbo.TrainingProgramVisits.VisitDate
FROM            dbo.TrainingProgram INNER JOIN
                         dbo.TrainingProgramVisits ON dbo.TrainingProgram.IdTrainingProgram = dbo.TrainingProgramVisits.IdTrainingProgram INNER JOIN
                         dbo.Resource ON dbo.TrainingProgramVisits.ResourceId = dbo.Resource.ResourceId
GROUP BY dbo.TrainingProgram.Name, dbo.Resource.Name, dbo.TrainingProgramVisits.VisitDate

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
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
         Begin Table = "TrainingProgram"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TrainingProgramVisits"
            Begin Extent = 
               Top = 6
               Left = 483
               Bottom = 135
               Right = 672
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Resource"
            Begin Extent = 
               Top = 6
               Left = 265
               Bottom = 135
               Right = 445
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TrainingProgramVisitsView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TrainingProgramVisitsView'
GO

-- Create dbo.TrainingProgramOnDemandVisitsView
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TrainingProgramOnDemandVisitsView]
AS
SELECT        dbo.TrainingProgramOnDemand.Name, dbo.Resource.Name AS Resource, COUNT(*) AS Visits, dbo.TrainingProgramOnDemandVisits.IdVisit, 
                         dbo.TrainingProgramOnDemandVisits.VisitDate
FROM            dbo.Resource INNER JOIN
                         dbo.TrainingProgramOnDemandVisits ON dbo.Resource.ResourceId = dbo.TrainingProgramOnDemandVisits.ResourceId INNER JOIN
                         dbo.TrainingProgramOnDemand ON 
                         dbo.TrainingProgramOnDemandVisits.IdTrainingProgramOnDemand = dbo.TrainingProgramOnDemand.IdTrainingProgramOnDemand
GROUP BY dbo.TrainingProgramOnDemand.Name, dbo.Resource.Name, dbo.TrainingProgramOnDemandVisits.IdVisit, dbo.TrainingProgramOnDemandVisits.VisitDate

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
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
         Begin Table = "Resource"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 218
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TrainingProgramOnDemandVisits"
            Begin Extent = 
               Top = 6
               Left = 544
               Bottom = 135
               Right = 794
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TrainingProgramOnDemand"
            Begin Extent = 
               Top = 6
               Left = 256
               Bottom = 135
               Right = 506
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TrainingProgramOnDemandVisitsView'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TrainingProgramOnDemandVisitsView'
GO

-- Drop Points columns
ALTER TABLE dbo.TrainingProgram
DROP COLUMN Points
GO

ALTER TABLE dbo.TrainingProgramOnDemand
DROP COLUMN Points
GO

ALTER TABLE dbo.GeneralTrainingProgram
DROP COLUMN Points
GO

-- Add AreaId
ALTER TABLE Suggestions ADD AreaId int NULL;
GO

UPDATE Suggestions SET AreaId = 0;
GO

ALTER TABLE [dbo].[Suggestions]  WITH CHECK ADD  CONSTRAINT [FK_Suggestions_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])
GO

ALTER TABLE [dbo].[Suggestions] CHECK CONSTRAINT [FK_Suggestions_Area]
GO

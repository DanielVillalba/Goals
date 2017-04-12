USE [CDPTrack]
GO

/****** Object:  Table [dbo].[TrainingProgramOnDemandDetails]    Script Date: 12/30/2014 11:32:09 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrainingProgramOnDemandDetails](
	[IdTrainingProgramOnDemandDetails] [int] IDENTITY(1,1) NOT NULL,
	[IdTrainingProgramOnDemand] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
 CONSTRAINT [PK_TrainingProgramOnDemandDetails] PRIMARY KEY CLUSTERED 
(
	[IdTrainingProgramOnDemandDetails] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TrainingProgramOnDemandDetails]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramOnDemandDetails_ProgressEnum] FOREIGN KEY([Status])
REFERENCES [dbo].[ProgressEnum] ([Id])
GO

ALTER TABLE [dbo].[TrainingProgramOnDemandDetails] CHECK CONSTRAINT [FK_TrainingProgramOnDemandDetails_ProgressEnum]
GO

ALTER TABLE [dbo].[TrainingProgramOnDemandDetails]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramOnDemandDetails_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO

ALTER TABLE [dbo].[TrainingProgramOnDemandDetails] CHECK CONSTRAINT [FK_TrainingProgramOnDemandDetails_Resource]
GO

ALTER TABLE [dbo].[TrainingProgramOnDemandDetails]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramOnDemandDetails_TrainingProgramOnDemand] FOREIGN KEY([IdTrainingProgramOnDemand])
REFERENCES [dbo].[TrainingProgramOnDemand] ([IdTrainingProgramOnDemand])
GO

ALTER TABLE [dbo].[TrainingProgramOnDemandDetails] CHECK CONSTRAINT [FK_TrainingProgramOnDemandDetails_TrainingProgramOnDemand]
GO


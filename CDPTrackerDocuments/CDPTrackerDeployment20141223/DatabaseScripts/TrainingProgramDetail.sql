USE [CDPTrack]
GO

/****** Object:  Table [dbo].[TrainingProgramDetails]    Script Date: 11/7/2014 9:19:27 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TrainingProgramDetails](
	[IdTrainingProgramDetails] [int] IDENTITY(1,1) NOT NULL,
	[IdTrainingProgram] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
 CONSTRAINT [PK_TrainingProgramDetails] PRIMARY KEY CLUSTERED 
(
	[IdTrainingProgramDetails] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TrainingProgramDetails]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramDetails_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO

ALTER TABLE [dbo].[TrainingProgramDetails] CHECK CONSTRAINT [FK_TrainingProgramDetails_Resource]
GO

ALTER TABLE [dbo].[TrainingProgramDetails]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramDetails_TrainingProgram] FOREIGN KEY([IdTrainingProgram])
REFERENCES [dbo].[TrainingProgram] ([IdTrainingProgram])
GO

ALTER TABLE [dbo].[TrainingProgramDetails] CHECK CONSTRAINT [FK_TrainingProgramDetails_TrainingProgram]
GO


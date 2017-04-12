USE [CDPTrack]
GO

/****** Object:  Table [dbo].[TrainingProgramVideo]    Script Date: 12/11/2014 8:52:18 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TrainingProgramVideo](
	[IdTrainingProgramVideo] [int] IDENTITY(1,1) NOT NULL,
	[IdTrainingProgram] [int] NOT NULL,
	[IdLocation] [int] NULL,
	[LinkVideo] [varchar](1000) NULL,
 CONSTRAINT [PK_TrainingProgramVideo] PRIMARY KEY CLUSTERED 
(
	[IdTrainingProgramVideo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[TrainingProgramVideo]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramVideo_Location] FOREIGN KEY([IdLocation])
REFERENCES [dbo].[Location] ([ID])
GO

ALTER TABLE [dbo].[TrainingProgramVideo] CHECK CONSTRAINT [FK_TrainingProgramVideo_Location]
GO

ALTER TABLE [dbo].[TrainingProgramVideo]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgramVideo_TrainingProgram] FOREIGN KEY([IdTrainingProgram])
REFERENCES [dbo].[TrainingProgram] ([IdTrainingProgram])
GO

ALTER TABLE [dbo].[TrainingProgramVideo] CHECK CONSTRAINT [FK_TrainingProgramVideo_TrainingProgram]
GO


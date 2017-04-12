USE [CDPTrack]
GO

/****** Object:  Table [dbo].[GeneralTrainingProgramVideo]    Script Date: 12/11/2014 8:52:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[GeneralTrainingProgramVideo](
	[IdGeneralTrainingProgramVideo] [int] IDENTITY(1,1) NOT NULL,
	[IdGenetalTrainingProgram] [int] NOT NULL,
	[IdLocation] [int] NULL,
	[LinkVideo] [varchar](1000) NULL,
 CONSTRAINT [PK_GeneralTrainingProgramVideo] PRIMARY KEY CLUSTERED 
(
	[IdGeneralTrainingProgramVideo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[GeneralTrainingProgramVideo]  WITH CHECK ADD  CONSTRAINT [FK_GeneralTrainingProgramVideo_GeneralTrainingProgram] FOREIGN KEY([IdGenetalTrainingProgram])
REFERENCES [dbo].[GeneralTrainingProgram] ([IdGeneralTrainingProgram])
GO

ALTER TABLE [dbo].[GeneralTrainingProgramVideo] CHECK CONSTRAINT [FK_GeneralTrainingProgramVideo_GeneralTrainingProgram]
GO

ALTER TABLE [dbo].[GeneralTrainingProgramVideo]  WITH CHECK ADD  CONSTRAINT [FK_GeneralTrainingProgramVideo_Location] FOREIGN KEY([IdLocation])
REFERENCES [dbo].[Location] ([ID])
GO

ALTER TABLE [dbo].[GeneralTrainingProgramVideo] CHECK CONSTRAINT [FK_GeneralTrainingProgramVideo_Location]
GO


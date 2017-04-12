USE [CDPTrack]
GO
/****** Object:  Table [dbo].[GeneralTrainingProgramDetails]    Script Date: 11/14/2014 12:19:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GeneralTrainingProgramDetails](
	[IdGeneralTrainingProgramDetails] [int] IDENTITY(1,1) NOT NULL,
	[IdGeneralTrainingProgram] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
 CONSTRAINT [PK_GeneralTrainingProgramDetails] PRIMARY KEY CLUSTERED 
(
	[IdGeneralTrainingProgramDetails] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[GeneralTrainingProgramDetails]  WITH CHECK ADD  CONSTRAINT [FK_GeneralTrainingProgramDetails_GeneralTrainingProgram] FOREIGN KEY([IdGeneralTrainingProgram])
REFERENCES [dbo].[GeneralTrainingProgram] ([IdGeneralTrainingProgram])
GO
ALTER TABLE [dbo].[GeneralTrainingProgramDetails] CHECK CONSTRAINT [FK_GeneralTrainingProgramDetails_GeneralTrainingProgram]
GO
ALTER TABLE [dbo].[GeneralTrainingProgramDetails]  WITH CHECK ADD  CONSTRAINT [FK_GeneralTrainingProgramDetails_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO
ALTER TABLE [dbo].[GeneralTrainingProgramDetails] CHECK CONSTRAINT [FK_GeneralTrainingProgramDetails_Resource]
GO

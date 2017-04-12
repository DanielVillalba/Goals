USE [CDPTrack]
GO
/****** Object:  Table [dbo].[GeneralTrainingProgram]    Script Date: 11/11/2014 11:16:45 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GeneralTrainingProgram](
	[IdGeneralTrainingProgram] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) NOT NULL,
	[Category] [int] NOT NULL,
	[Points] [int] NOT NULL,
	[Link] [varchar](1000) NULL,
 CONSTRAINT [PK_GeneralTrainingProgram] PRIMARY KEY CLUSTERED 
(
	[IdGeneralTrainingProgram] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[GeneralTrainingProgram]  WITH CHECK ADD  CONSTRAINT [FK_GeneralTrainingProgram_TrainingProgramCategory] FOREIGN KEY([Category])
REFERENCES [dbo].[TrainingProgramCategory] ([IdTrainingProgramCategory])
GO
ALTER TABLE [dbo].[GeneralTrainingProgram] CHECK CONSTRAINT [FK_GeneralTrainingProgram_TrainingProgramCategory]
GO

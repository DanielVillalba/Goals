USE [CDPTrack]
GO
/****** Object:  Table [dbo].[TrainingProgram]    Script Date: 11/3/2014 12:47:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TrainingProgram](
	[IdTrainingProgram] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](1000) NOT NULL,
	[Position] [int] NOT NULL,
	[Category] [int] NOT NULL,
	[Points] [int] NOT NULL,
	[Link] [varchar](1000) NULL,
 CONSTRAINT [PK_TrainingProgram] PRIMARY KEY CLUSTERED 
(
	[IdTrainingProgram] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[TrainingProgram]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgram_Position] FOREIGN KEY([Position])
REFERENCES [dbo].[Position] ([PositionId])
GO
ALTER TABLE [dbo].[TrainingProgram] CHECK CONSTRAINT [FK_TrainingProgram_Position]
GO
ALTER TABLE [dbo].[TrainingProgram]  WITH CHECK ADD  CONSTRAINT [FK_TrainingProgram_TrainingProgramCategory] FOREIGN KEY([Category])
REFERENCES [dbo].[TrainingProgramCategory] ([IdTrainingProgramCategory])
GO
ALTER TABLE [dbo].[TrainingProgram] CHECK CONSTRAINT [FK_TrainingProgram_TrainingProgramCategory]
GO

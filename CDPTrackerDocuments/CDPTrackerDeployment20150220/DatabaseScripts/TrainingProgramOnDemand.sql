USE [CDPTrack]
GO

/****** Object:  Table [dbo].[TrainingProgramOnDemand]    Script Date: 12/30/2014 11:31:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TrainingProgramOnDemand](
	[IdTrainingProgramOnDemand] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NULL,
	[Points] [int] NULL,
	[Link] [varchar](1000) NULL,
	[StartDate] [date] NULL,
	[FinishDate] [date] NULL,
	[Enable] [bit] NULL,
 CONSTRAINT [PK_TrainingProgramOnDemand] PRIMARY KEY CLUSTERED 
(
	[IdTrainingProgramOnDemand] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


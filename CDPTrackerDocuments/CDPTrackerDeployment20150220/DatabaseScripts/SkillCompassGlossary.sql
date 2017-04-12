USE [CDPTrack]
GO

/****** Object:  Table [dbo].[SkillCompassGlossary]    Script Date: 1/21/2015 9:01:15 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SkillCompassGlossary](
	[SkillCompassGlossaryId] [int] IDENTITY(1,1) NOT NULL,
	[AreaId] [int] NOT NULL,
	[Name] [varchar](100) NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_SkillCompassGlossary] PRIMARY KEY CLUSTERED 
(
	[SkillCompassGlossaryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[SkillCompassGlossary]  WITH CHECK ADD  CONSTRAINT [FK_SkillCompassGlossary_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])
GO

ALTER TABLE [dbo].[SkillCompassGlossary] CHECK CONSTRAINT [FK_SkillCompassGlossary_Area]
GO



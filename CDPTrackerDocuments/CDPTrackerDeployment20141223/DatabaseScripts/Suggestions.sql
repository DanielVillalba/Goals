USE [CDPTrack]
GO

/****** Object:  Table [dbo].[Suggestions]    Script Date: 10/30/2014 11:29:23 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Suggestions](
	[SuggestionId] [int] IDENTITY(1,1) NOT NULL,
	[PositionId] [int] NOT NULL,
	[TechnologyId] [int] NOT NULL,
	[LevelId] [int] NOT NULL,
	[Topic] [varchar](1000) NOT NULL,
	[Source] [varchar](1000) NOT NULL,
 CONSTRAINT [PK_Suggestions] PRIMARY KEY CLUSTERED 
(
	[SuggestionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Suggestions]  WITH CHECK ADD  CONSTRAINT [FK_Suggestions_Level] FOREIGN KEY([LevelId])
REFERENCES [dbo].[Level] ([LevelId])
GO

ALTER TABLE [dbo].[Suggestions] CHECK CONSTRAINT [FK_Suggestions_Level]
GO

ALTER TABLE [dbo].[Suggestions]  WITH CHECK ADD  CONSTRAINT [FK_Suggestions_Position] FOREIGN KEY([PositionId])
REFERENCES [dbo].[Position] ([PositionId])
GO

ALTER TABLE [dbo].[Suggestions] CHECK CONSTRAINT [FK_Suggestions_Position]
GO

ALTER TABLE [dbo].[Suggestions]  WITH CHECK ADD  CONSTRAINT [FK_Suggestions_Technologies] FOREIGN KEY([TechnologyId])
REFERENCES [dbo].[Technologies] ([TechnologyId])
GO

ALTER TABLE [dbo].[Suggestions] CHECK CONSTRAINT [FK_Suggestions_Technologies]
GO


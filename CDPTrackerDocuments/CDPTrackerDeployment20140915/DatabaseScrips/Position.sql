USE [CDPTrack]
GO
/****** Object:  Table [dbo].[Position]    Script Date: 8/11/2014 9:16:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Position](
	[PositionId] [int] IDENTITY(1,1) NOT NULL,
	[PositionName] [varchar](200) NOT NULL,
	[NextPosition] [int] NULL,
	[AreaId] [int] NOT NULL,
 CONSTRAINT [PK_Position] PRIMARY KEY CLUSTERED 
(
	[PositionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])
GO
ALTER TABLE [dbo].[Position] CHECK CONSTRAINT [FK_Position_Area]
GO
ALTER TABLE [dbo].[Position]  WITH CHECK ADD  CONSTRAINT [FK_Position_Position] FOREIGN KEY([NextPosition])
REFERENCES [dbo].[Position] ([PositionId])
GO
ALTER TABLE [dbo].[Position] CHECK CONSTRAINT [FK_Position_Position]
GO

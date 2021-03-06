USE [CDPTrack]
GO
/****** Object:  Table [dbo].[Objective]    Script Date: 8/11/2014 10:28:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Objective](
	[ObjectiveId] [int] IDENTITY(1,1) NOT NULL,
	[Objective] [nvarchar](1000) NULL,
	[CategoryId] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
 CONSTRAINT [PK_Objective] PRIMARY KEY CLUSTERED 
(
	[ObjectiveId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Objective]  WITH CHECK ADD  CONSTRAINT [FK_Objective_Category] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Category] ([CategoryId])
GO
ALTER TABLE [dbo].[Objective] CHECK CONSTRAINT [FK_Objective_Category]
GO
ALTER TABLE [dbo].[Objective]  WITH CHECK ADD  CONSTRAINT [FK_Objective_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO
ALTER TABLE [dbo].[Objective] CHECK CONSTRAINT [FK_Objective_Resource]
GO

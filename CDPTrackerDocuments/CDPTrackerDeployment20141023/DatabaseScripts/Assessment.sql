USE [CDPTrack]
GO
/****** Object:  Table [dbo].[Assessment]    Script Date: 9/19/2014 8:54:11 AM ******/
SET ANSI_NULLS ON
USE [CDPTrack]
GO
/****** Object:  Table [dbo].[Assessment]    Script Date: 9/23/2014 12:40:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Assessment](
	[AssessmentId] [int] IDENTITY(1,1) NOT NULL,
	[Description] [varchar](1000) NOT NULL,
	[AssessmentCategory] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[Term] [int] NOT NULL,
 CONSTRAINT [PK_Assessment] PRIMARY KEY CLUSTERED 
(
	[AssessmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Assessment]  WITH CHECK ADD  CONSTRAINT [FK_Assessment_AssessmentCategory] FOREIGN KEY([AssessmentCategory])
REFERENCES [dbo].[AssessmentCategory] ([AssessmentCategoryId])
GO
ALTER TABLE [dbo].[Assessment] CHECK CONSTRAINT [FK_Assessment_AssessmentCategory]
GO
ALTER TABLE [dbo].[Assessment]  WITH CHECK ADD  CONSTRAINT [FK_Assessment_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO
ALTER TABLE [dbo].[Assessment] CHECK CONSTRAINT [FK_Assessment_Resource]
GO
ALTER TABLE [dbo].[Assessment]  WITH CHECK ADD  CONSTRAINT [FK_Assessment_Term] FOREIGN KEY([Term])
REFERENCES [dbo].[Term] ([TermId])
GO
ALTER TABLE [dbo].[Assessment] CHECK CONSTRAINT [FK_Assessment_Term]
GO


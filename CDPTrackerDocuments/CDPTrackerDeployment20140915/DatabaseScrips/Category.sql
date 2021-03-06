USE [CDPTrack]
GO
/****** Object:  Table [dbo].[Category]    Script Date: 8/11/2014 9:47:06 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Category](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Category] [nvarchar](50) NULL,
 CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Category] ON 

INSERT [dbo].[Category] ([CategoryId], [Category]) VALUES (1, N'Technical')
INSERT [dbo].[Category] ([CategoryId], [Category]) VALUES (2, N'Soft Skills')
INSERT [dbo].[Category] ([CategoryId], [Category]) VALUES (3, N'Organizational')
SET IDENTITY_INSERT [dbo].[Category] OFF

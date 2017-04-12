USE [CDPTrack]

ALTER TABLE Position DROP CONSTRAINT FK_Position_Position
ALTER TABLE Position DROP COLUMN NextPosition
ALTER TABLE Position ADD Description Varchar(1000)

GO
/****** Object:  Table [dbo].[PositionsHierarchy]    Script Date: 10/3/2014 8:51:25 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PositionsHierarchy](
	[PositionHierarchyId] [int] IDENTITY(1,1) NOT NULL,
	[PositionId] [int] NOT NULL,
	[NextPosition] [int] NOT NULL,
 CONSTRAINT [PK_PositionsHierarchy] PRIMARY KEY CLUSTERED 
(
	[PositionHierarchyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[PositionsHierarchy] ON 

GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (1, 1, 2)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (2, 2, 3)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (3, 3, 10)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (4, 3, 5)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (5, 5, 7)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (6, 5, 10)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (7, 7, 8)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (8, 8, 9)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (9, 10, 11)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (10, 11, 12)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (11, 12, 13)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (12, 14, 15)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (13, 15, 16)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (14, 16, 17)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (15, 17, 18)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (16, 19, 18)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (17, 20, 18)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (18, 21, 22)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (19, 22, 23)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (20, 23, 24)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (21, 24, 25)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (22, 25, 26)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (23, 26, 27)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (24, 27, 28)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (25, 28, 29)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (27, 30, 31)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (28, 31, 32)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (29, 32, 33)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (30, 33, 34)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (31, 34, 35)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (32, 36, 37)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (33, 37, 38)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (34, 38, 40)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (35, 38, 41)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (36, 40, 42)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (37, 41, 43)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (38, 42, 44)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (39, 43, 42)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (40, 44, 45)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (41, 46, 48)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (42, 46, 49)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (43, 48, 49)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (44, 49, 48)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (45, 49, 63)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (46, 48, 52)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (47, 52, 53)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (48, 53, 54)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (49, 54, 55)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (50, 53, 58)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (51, 57, 55)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (52, 58, 59)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (53, 60, 61)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (54, 61, 62)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (55, 63, 64)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (56, 64, 57)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (57, 64, 58)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (58, 66, 70)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (59, 66, 84)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (60, 66, 85)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (61, 66, 91)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (62, 70, 71)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (63, 71, 72)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (64, 72, 74)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (65, 72, 77)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (66, 74, 75)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (67, 75, 76)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (68, 77, 79)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (69, 77, 81)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (70, 79, 76)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (71, 80, 77)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (72, 81, 82)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (73, 82, 83)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (74, 83, 98)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (75, 84, 81)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (76, 85, 86)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (77, 86, 87)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (78, 87, 88)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (79, 88, 81)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (74, 88, 90)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (81, 91, 92)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (82, 92, 93)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (83, 93, 94)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (84, 81, 96)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (85, 96, 97)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (87, 97, 98)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (88, 99, 100)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (89, 100, 101)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (90, 101, 102)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (91, 103, 105)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (92, 103, 108)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (93, 105, 106)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (94, 106, 107)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (95, 107, 102)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (96, 108, 103)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (97, 108, 110)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (98, 110, 106)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (99, 105, 110)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (100, 110, 105)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (101, 113, 114)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (102, 114, 115)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (103, 115, 116)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (104, 115, 118)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (105, 118, 119)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (106, 119, 120)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (107, 115, 122)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (108, 122, 123)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (109, 123, 124)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (110, 125, 127)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (111, 125, 135)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (112, 127, 128)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (113, 128, 116)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (114, 128, 131)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (115, 131, 132)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (116, 132, 133)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (117, 133, 134)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (118, 125, 118)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (119, 125, 139)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (120, 118, 139)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (121, 139, 119)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (122, 119, 120)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (123, 142, 143)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (124, 143, 144)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (125, 144, 145)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (126, 145, 146)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (127, 146, 147)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (128, 147, 148)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (129, 148, 149)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (130, 149, 150)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (131, 150, 151)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (132, 151, 152)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (133, 152, 153)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (134, 154, 155)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (135, 155, 156)
GO
INSERT [dbo].[PositionsHierarchy] ([PositionHierarchyId], [PositionId], [NextPosition]) VALUES (136, 156, 157)
GO
SET IDENTITY_INSERT [dbo].[PositionsHierarchy] OFF
GO
ALTER TABLE [dbo].[PositionsHierarchy]  WITH CHECK ADD  CONSTRAINT [FK_PositionsHierarchy_Position] FOREIGN KEY([PositionId])
REFERENCES [dbo].[Position] ([PositionId])
GO
ALTER TABLE [dbo].[PositionsHierarchy] CHECK CONSTRAINT [FK_PositionsHierarchy_Position]
GO
ALTER TABLE [dbo].[PositionsHierarchy]  WITH CHECK ADD  CONSTRAINT [FK_PositionsHierarchy_Position1] FOREIGN KEY([NextPosition])
REFERENCES [dbo].[Position] ([PositionId])
GO
ALTER TABLE [dbo].[PositionsHierarchy] CHECK CONSTRAINT [FK_PositionsHierarchy_Position1]
GO

DELETE FROM Position WHERE PositionId NOT IN (SELECT MIN(PositionId) FROM Position GROUP BY PositionName);

/*************************/

/*********************************
IF SOMETHING GOES WRONG HELP YOURSELF
WITH THE NEXT QUERIES:

SELECT COUNT(PositionName) as times,Position.PositionName FROM Position GROUP BY PositionName order by times

SELECT Position.PositionId, Position.PositionName,PositionsHierarchy.PositionHierarchyId FROM Position INNER JOIN PositionsHierarchy
ON Position.PositionId = PositionsHierarchy.PositionId


***********************************/

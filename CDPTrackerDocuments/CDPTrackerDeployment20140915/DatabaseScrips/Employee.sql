USE [CDPTrack]
GO
/****** Object:  Table [dbo].[Employee]   Script Date: 8/11/2014 9:10:14 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[Employee]
	ADD CurrentPositionID INT NULL, AspiringPositionID INT NULL

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Position] FOREIGN KEY([CurrentPositionID])
REFERENCES [dbo].[Position] ([PositionId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Position]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Position1] FOREIGN KEY([AspiringPositionID])
REFERENCES [dbo].[Position] ([PositionId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Position1]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Resource] FOREIGN KEY([ResourceId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Resource]
GO
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Manager_Resource] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Resource] ([ResourceId])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Manager_Resource]
GO

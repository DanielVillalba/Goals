USE CDPTrack

ALTER TABLE Employee ADD Type varchar(30) NULL;

ALTER TABLE Employee ADD AreaId int NULL;

ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Area] FOREIGN KEY([AreaId])
REFERENCES [dbo].[Area] ([AreaId])


USE [CDPTrack]
GO
ALTER TABLE [dbo].[GeneralTrainingProgramDetails]  WITH CHECK ADD  CONSTRAINT [FK_GeneralTrainingProgramDetails_ProgressEnum] FOREIGN KEY([Status])
REFERENCES [dbo].[ProgressEnum] ([Id])
CREATE TABLE [dbo].[TrainingProgramVideo] (
    [IdTrainingProgramVideo] INT            IDENTITY (1, 1) NOT NULL,
    [IdTrainingProgram]      INT            NOT NULL,
    [IdLocation]             INT            NULL,
    [LinkVideo]              VARCHAR (1000) NULL,
    CONSTRAINT [PK_TrainingProgramVideo] PRIMARY KEY CLUSTERED ([IdTrainingProgramVideo] ASC),
    CONSTRAINT [FK_TrainingProgramVideo_Location] FOREIGN KEY ([IdLocation]) REFERENCES [dbo].[Location] ([ID]),
    CONSTRAINT [FK_TrainingProgramVideo_TrainingProgram] FOREIGN KEY ([IdTrainingProgram]) REFERENCES [dbo].[TrainingProgram] ([IdTrainingProgram])
);


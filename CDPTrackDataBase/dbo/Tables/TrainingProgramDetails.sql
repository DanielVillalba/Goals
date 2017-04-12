CREATE TABLE [dbo].[TrainingProgramDetails] (
    [IdTrainingProgramDetails] INT IDENTITY (1, 1) NOT NULL,
    [IdTrainingProgram]        INT NOT NULL,
    [Status]                   INT NOT NULL,
    [ResourceId]               INT NOT NULL,
    CONSTRAINT [PK_TrainingProgramDetails] PRIMARY KEY CLUSTERED ([IdTrainingProgramDetails] ASC),
    CONSTRAINT [FK_TrainingProgramDetails_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId]),
    CONSTRAINT [FK_TrainingProgramDetails_TrainingProgram] FOREIGN KEY ([IdTrainingProgram]) REFERENCES [dbo].[TrainingProgram] ([IdTrainingProgram])
);


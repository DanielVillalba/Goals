CREATE TABLE [dbo].[TrainingProgramVisits] (
    [IdVisit]           INT  IDENTITY (1, 1) NOT NULL,
    [IdTrainingProgram] INT  NOT NULL,
    [ResourceId]        INT  NOT NULL,
    [VisitDate]         DATE NOT NULL,
    CONSTRAINT [PK_TrainingProgramVisits] PRIMARY KEY CLUSTERED ([IdVisit] ASC),
    CONSTRAINT [FK_TrainingProgramVisits_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId]),
    CONSTRAINT [FK_TrainingProgramVisits_TrainingProgram] FOREIGN KEY ([IdTrainingProgram]) REFERENCES [dbo].[TrainingProgram] ([IdTrainingProgram])
);


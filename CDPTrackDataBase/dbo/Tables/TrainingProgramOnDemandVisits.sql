CREATE TABLE [dbo].[TrainingProgramOnDemandVisits] (
    [IdVisit]                   INT  IDENTITY (1, 1) NOT NULL,
    [IdTrainingProgramOnDemand] INT  NOT NULL,
    [ResourceId]                INT  NOT NULL,
    [VisitDate]                 DATE NOT NULL,
    CONSTRAINT [PK_TrainingProgramOnDemandVisits] PRIMARY KEY CLUSTERED ([IdVisit] ASC),
    CONSTRAINT [FK_TrainingProgramOnDemandVisits_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId]),
    CONSTRAINT [FK_TrainingProgramOnDemandVisits_TrainingProgramOnDemand] FOREIGN KEY ([IdTrainingProgramOnDemand]) REFERENCES [dbo].[TrainingProgramOnDemand] ([IdTrainingProgramOnDemand])
);


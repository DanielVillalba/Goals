CREATE TABLE [dbo].[TrainingProgramOnDemandDetails] (
    [IdTrainingProgramOnDemandDetails] INT IDENTITY (1, 1) NOT NULL,
    [IdTrainingProgramOnDemand]        INT NOT NULL,
    [Status]                           INT NOT NULL,
    [ResourceId]                       INT NOT NULL,
    CONSTRAINT [PK_TrainingProgramOnDemandDetails] PRIMARY KEY CLUSTERED ([IdTrainingProgramOnDemandDetails] ASC),
    CONSTRAINT [FK_TrainingProgramOnDemandDetails_ProgressEnum] FOREIGN KEY ([Status]) REFERENCES [dbo].[ProgressEnum] ([Id]),
    CONSTRAINT [FK_TrainingProgramOnDemandDetails_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId]),
    CONSTRAINT [FK_TrainingProgramOnDemandDetails_TrainingProgramOnDemand] FOREIGN KEY ([IdTrainingProgramOnDemand]) REFERENCES [dbo].[TrainingProgramOnDemand] ([IdTrainingProgramOnDemand])
);


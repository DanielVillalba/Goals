CREATE TABLE [dbo].[GeneralTrainingProgramDetails] (
    [IdGeneralTrainingProgramDetails] INT IDENTITY (1, 1) NOT NULL,
    [IdGeneralTrainingProgram]        INT NOT NULL,
    [Status]                          INT NOT NULL,
    [ResourceId]                      INT NOT NULL,
    CONSTRAINT [PK_GeneralTrainingProgramDetails] PRIMARY KEY CLUSTERED ([IdGeneralTrainingProgramDetails] ASC),
    CONSTRAINT [FK_GeneralTrainingProgramDetails_GeneralTrainingProgram] FOREIGN KEY ([IdGeneralTrainingProgram]) REFERENCES [dbo].[GeneralTrainingProgram] ([IdGeneralTrainingProgram]),
    CONSTRAINT [FK_GeneralTrainingProgramDetails_ProgressEnum] FOREIGN KEY ([Status]) REFERENCES [dbo].[ProgressEnum] ([Id]),
    CONSTRAINT [FK_GeneralTrainingProgramDetails_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId])
);


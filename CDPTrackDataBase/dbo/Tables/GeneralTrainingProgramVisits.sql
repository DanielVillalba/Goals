CREATE TABLE [dbo].[GeneralTrainingProgramVisits] (
    [IdVisit]                  INT  IDENTITY (1, 1) NOT NULL,
    [IdGeneralTrainingProgram] INT  NOT NULL,
    [ResourceId]               INT  NOT NULL,
    [VisitDate]                DATE NOT NULL,
    CONSTRAINT [PK_GeneralTrainingProgramVisits] PRIMARY KEY CLUSTERED ([IdVisit] ASC),
    CONSTRAINT [FK_GeneralTrainingProgramVisits_GeneralTrainingProgram] FOREIGN KEY ([IdGeneralTrainingProgram]) REFERENCES [dbo].[GeneralTrainingProgram] ([IdGeneralTrainingProgram]),
    CONSTRAINT [FK_GeneralTrainingProgramVisits_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId])
);


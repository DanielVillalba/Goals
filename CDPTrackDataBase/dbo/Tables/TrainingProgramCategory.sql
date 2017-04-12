CREATE TABLE [dbo].[TrainingProgramCategory] (
    [IdTrainingProgramCategory] INT            IDENTITY (1, 1) NOT NULL,
    [Name]                      VARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_TrainingProgramCategory] PRIMARY KEY CLUSTERED ([IdTrainingProgramCategory] ASC)
);


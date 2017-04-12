CREATE TABLE [dbo].[TrainingProgramOnDemand] (
    [IdTrainingProgramOnDemand] INT            IDENTITY (1, 1) NOT NULL,
    [Name]                      VARCHAR (100)  NULL,
    [Link]                      VARCHAR (1000) NULL,
    [StartDate]                 DATE           NULL,
    [FinishDate]                DATE           NULL,
    [Enable]                    BIT            NULL,
    CONSTRAINT [PK_TrainingProgramOnDemand] PRIMARY KEY CLUSTERED ([IdTrainingProgramOnDemand] ASC)
);


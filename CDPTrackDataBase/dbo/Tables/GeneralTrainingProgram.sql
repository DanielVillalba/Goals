CREATE TABLE [dbo].[GeneralTrainingProgram] (
    [IdGeneralTrainingProgram] INT            IDENTITY (1, 1) NOT NULL,
    [Name]                     VARCHAR (1000) NOT NULL,
    [Category]                 INT            NOT NULL,
    [Link]                     VARCHAR (1000) NULL,
    [Enabled]                  BIT            DEFAULT ('True') NOT NULL,
    [StartDate]                DATE           NULL,
    [FinishDate]               DATE           NULL,
    CONSTRAINT [PK_GeneralTrainingProgram] PRIMARY KEY CLUSTERED ([IdGeneralTrainingProgram] ASC),
    CONSTRAINT [FK_GeneralTrainingProgram_TrainingProgramCategory] FOREIGN KEY ([Category]) REFERENCES [dbo].[TrainingProgramCategory] ([IdTrainingProgramCategory])
);


CREATE TABLE [dbo].[TrainingProgram] (
    [IdTrainingProgram] INT            IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (1000) NOT NULL,
    [Position]          INT            NOT NULL,
    [Category]          INT            NOT NULL,
    [Link]              VARCHAR (1000) NULL,
    [Enable]            BIT            DEFAULT ('True') NULL,
    [StartDate]         DATE           NULL,
    [FinishDate]        DATE           NULL,
    CONSTRAINT [PK_TrainingProgram] PRIMARY KEY CLUSTERED ([IdTrainingProgram] ASC),
    CONSTRAINT [FK_TrainingProgram_Position] FOREIGN KEY ([Position]) REFERENCES [dbo].[Position] ([PositionId]),
    CONSTRAINT [FK_TrainingProgram_TrainingProgramCategory] FOREIGN KEY ([Category]) REFERENCES [dbo].[TrainingProgramCategory] ([IdTrainingProgramCategory])
);


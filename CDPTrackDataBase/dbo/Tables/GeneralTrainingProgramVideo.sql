CREATE TABLE [dbo].[GeneralTrainingProgramVideo] (
    [IdGeneralTrainingProgramVideo] INT            IDENTITY (1, 1) NOT NULL,
    [IdGenetalTrainingProgram]      INT            NOT NULL,
    [IdLocation]                    INT            NULL,
    [LinkVideo]                     VARCHAR (1000) NULL,
    CONSTRAINT [PK_GeneralTrainingProgramVideo] PRIMARY KEY CLUSTERED ([IdGeneralTrainingProgramVideo] ASC),
    CONSTRAINT [FK_GeneralTrainingProgramVideo_GeneralTrainingProgram] FOREIGN KEY ([IdGenetalTrainingProgram]) REFERENCES [dbo].[GeneralTrainingProgram] ([IdGeneralTrainingProgram]),
    CONSTRAINT [FK_GeneralTrainingProgramVideo_Location] FOREIGN KEY ([IdLocation]) REFERENCES [dbo].[Location] ([ID])
);


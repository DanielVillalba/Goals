CREATE TABLE [dbo].[SurveyResource] (
    [SurveyResourceId]    INT  IDENTITY (1, 1) NOT NULL,
    [ResourceId]          INT  NOT NULL,
    [SurveyId]            INT  NOT NULL,
    [DateAnswered]        DATE NOT NULL,
    [SurveyType]          INT  NOT NULL,
    [ResourceEvaluatedId] INT  NULL,
    CONSTRAINT [PK_SurveyResource] PRIMARY KEY CLUSTERED ([SurveyResourceId] ASC),
    CONSTRAINT [FK_SurveyResource_Survey] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Survey] ([SurveyId])
);


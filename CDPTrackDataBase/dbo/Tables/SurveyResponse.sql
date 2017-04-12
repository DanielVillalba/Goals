CREATE TABLE [dbo].[SurveyResponse] (
    [SurveyResponseId] INT           IDENTITY (1, 1) NOT NULL,
    [QuestionId]       INT           NOT NULL,
    [ResourceId]       INT           NOT NULL,
    [ResponseId]       INT           NULL,
    [ResponseText]     VARCHAR (555) NULL,
    [SurveyResourceId] INT           NOT NULL,
    CONSTRAINT [PK_SurveyResponse] PRIMARY KEY CLUSTERED ([SurveyResponseId] ASC),
    CONSTRAINT [FK_SurveyResponse_Question] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Question] ([QuestionId]),
    CONSTRAINT [FK_SurveyResponse_SurveyResource] FOREIGN KEY ([SurveyResourceId]) REFERENCES [dbo].[SurveyResource] ([SurveyResourceId])
);


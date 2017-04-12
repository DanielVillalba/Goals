CREATE TABLE [dbo].[Question] (
    [QuestionId]         INT           NOT NULL,
    [SurveyId]           INT           NOT NULL,
    [Text]               VARCHAR (200) NOT NULL,
    [Updated]            DATETIME      NOT NULL,
    [Sequence]           INT           NOT NULL,
    [QuestionType]       INT           NOT NULL,
    [QuestionChild]      INT           NULL,
    [Required]           INT           NULL,
    [DisplayWhenValue]   INT           NULL,
    [VisibleForEmployee] INT           NULL,
    CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ([QuestionId] ASC),
    CONSTRAINT [FK_Question_Survey] FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Survey] ([SurveyId])
);


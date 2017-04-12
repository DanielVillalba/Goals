CREATE TABLE [dbo].[Survey] (
    [SurveyId]         INT          NOT NULL,
    [Name]             VARCHAR (50) NULL,
    [Description]      VARCHAR (50) NOT NULL,
    [SurveyType]       INT          NULL,
    [CreatedBy]        INT          NULL,
    [CreatedTimeStamp] DATETIME     NULL,
    [Quarter]          INT          NULL,
    [Year]             INT          NULL,
    CONSTRAINT [PK_Survey] PRIMARY KEY CLUSTERED ([SurveyId] ASC)
);


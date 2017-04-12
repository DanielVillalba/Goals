CREATE TABLE [dbo].[QuestionResponse] (
    [QuestionId]         INT NOT NULL,
    [ResponseId]         INT NOT NULL,
    [QuestionResponseId] INT NOT NULL,
    CONSTRAINT [PK_QuestionResponse] PRIMARY KEY CLUSTERED ([QuestionResponseId] ASC),
    CONSTRAINT [FK_QuestionResponse_Question] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Question] ([QuestionId]),
    CONSTRAINT [FK_QuestionResponse_Response] FOREIGN KEY ([ResponseId]) REFERENCES [dbo].[Response] ([ResponseId])
);


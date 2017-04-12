CREATE TABLE [dbo].[Response] (
    [ResponseId] INT            NOT NULL,
    [Answer]     VARCHAR (1000) NULL,
    CONSTRAINT [PK_Response] PRIMARY KEY CLUSTERED ([ResponseId] ASC)
);


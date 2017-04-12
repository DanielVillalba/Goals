CREATE TABLE [dbo].[AppError] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [errcode] INT            NOT NULL,
    [message] NVARCHAR (150) NOT NULL,
    CONSTRAINT [PK_AppError] PRIMARY KEY CLUSTERED ([Id] ASC)
);


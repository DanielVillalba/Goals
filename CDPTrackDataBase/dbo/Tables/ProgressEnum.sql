CREATE TABLE [dbo].[ProgressEnum] (
    [Id]    INT           NOT NULL,
    [Label] NVARCHAR (50) NULL,
    CONSTRAINT [PK_ProgressEnum] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UX_ProgressEnum_Label] UNIQUE NONCLUSTERED ([Label] ASC)
);


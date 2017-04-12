CREATE TABLE [dbo].[Group_SectionAccess] (
    [id]      BIGINT        NOT NULL,
    [GroupId] INT           NOT NULL,
    [Section] NVARCHAR (50) NOT NULL,
    [Allow]   BIT           NOT NULL,
    CONSTRAINT [PK_Group_SectionAccess] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Group_SectionAccess_Groups] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[Groups] ([GroupId])
);


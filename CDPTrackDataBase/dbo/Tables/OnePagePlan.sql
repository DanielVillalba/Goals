CREATE TABLE [dbo].[OnePagePlan] (
    [OnePagePlanId]      INT           IDENTITY (1, 1) NOT NULL,
    [Quarter]            INT           NULL,
    [year]               INT           NULL,
    [month1_CoreValueId] INT           NULL,
    [month2_CoreValueId] INT           NULL,
    [month3_CoreValueId] INT           NULL,
    [SG]                 VARCHAR (150) NULL,
    [G]                  VARCHAR (150) NULL,
    [R]                  VARCHAR (150) NULL,
    [Y]                  VARCHAR (150) NULL,
    [ResourceId]         INT           NOT NULL,
    [QuarterId]          INT           NOT NULL,
    CONSTRAINT [PK_OnePagePlanId] PRIMARY KEY CLUSTERED ([OnePagePlanId] ASC),
    CONSTRAINT [FK__Resource__QuarterId] FOREIGN KEY ([QuarterId]) REFERENCES [dbo].[Quarter] ([QuarterId]),
    CONSTRAINT [FK__Resource__ResourceId] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId])
);






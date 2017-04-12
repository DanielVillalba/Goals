CREATE TABLE [dbo].[QuarterlyPriorities] (
    [QuarterlyPrioritiesId] INT           IDENTITY (1, 1) NOT NULL,
    [Quarter]               INT           NULL,
    [Year]                  INT           NULL,
    [ResourceId]            INT           NULL,
    [CreationDate]          DATE          NULL,
    [OnePagePlanId]         INT           NULL,
    [KeyIssue]              VARCHAR (512) NULL,
    [ClosingComment]        VARCHAR (512) NULL,
    [CEOComment]            VARCHAR (512) NULL,
    [ManagerComment]        VARCHAR (512) NULL,
    [QuarterId]             INT           NOT NULL,
    CONSTRAINT [PK_QuarterlyPriorities] PRIMARY KEY CLUSTERED ([QuarterlyPrioritiesId] ASC),
    CONSTRAINT [FK_QuarterlyPriorities_OnePagePlan] FOREIGN KEY ([OnePagePlanId]) REFERENCES [dbo].[OnePagePlan] ([OnePagePlanId]),
    CONSTRAINT [FK_QuarterlyPriorities_Quarter] FOREIGN KEY ([QuarterId]) REFERENCES [dbo].[Quarter] ([QuarterId])
);






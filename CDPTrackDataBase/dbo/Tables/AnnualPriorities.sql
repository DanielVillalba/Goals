CREATE TABLE [dbo].[AnnualPriorities] (
    [AnnualPrioritiesId] INT            IDENTITY (1, 1) NOT NULL,
    [OnePagePlanId]      INT            NOT NULL,
    [AnnualPriorities]   NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_AnnualPriorities] PRIMARY KEY CLUSTERED ([AnnualPrioritiesId] ASC),
    CONSTRAINT [FK_AnnualPriorities_OnePagePlanId] FOREIGN KEY ([OnePagePlanId]) REFERENCES [dbo].[OnePagePlan] ([OnePagePlanId])
);


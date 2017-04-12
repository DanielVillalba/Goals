CREATE TABLE [dbo].[KeyThrusts] (
    [KeyThrustsId]  INT            IDENTITY (1, 1) NOT NULL,
    [OnePagePlanId] INT            NOT NULL,
    [KeyThrust]     NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_KeyThrust] PRIMARY KEY CLUSTERED ([KeyThrustsId] ASC),
    CONSTRAINT [FK_OnePagePlanId] FOREIGN KEY ([OnePagePlanId]) REFERENCES [dbo].[OnePagePlan] ([OnePagePlanId])
);


CREATE TABLE [dbo].[Employee] (
    [ResourceId]         INT             NOT NULL,
    [ManagerId]          INT             NULL,
    [CurrentPosition]    NVARCHAR (1000) NULL,
    [AspiringPosition]   NVARCHAR (1000) NULL,
    [CurrentPositionID]  INT             NULL,
    [AspiringPositionID] INT             NULL,
    [ProjectId]          INT             NULL,
    [Type]               VARCHAR (30)    NULL,
    [AreaId]             INT             NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED ([ResourceId] ASC),
    CONSTRAINT [FK_Employee_Area] FOREIGN KEY ([AreaId]) REFERENCES [dbo].[Area] ([AreaId]),
    CONSTRAINT [FK_Employee_Position] FOREIGN KEY ([CurrentPositionID]) REFERENCES [dbo].[Position] ([PositionId]),
    CONSTRAINT [FK_Employee_Position1] FOREIGN KEY ([AspiringPositionID]) REFERENCES [dbo].[Position] ([PositionId]),
    CONSTRAINT [FK_Employee_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId]),
    CONSTRAINT [FK_Employee_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [dbo].[Resource] ([ResourceId])
);


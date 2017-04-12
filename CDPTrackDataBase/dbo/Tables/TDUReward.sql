CREATE TABLE [dbo].[TDUReward] (
    [TDURewardId]        INT      IDENTITY (1, 1) NOT NULL,
    [resourceId]         INT      NOT NULL,
    [StartingQuarter]    INT      NOT NULL,
    [EndingQuarter]      INT      NOT NULL,
    [StartingYear]       INT      NOT NULL,
    [EndingYear]         INT      NOT NULL,
    [DatetoLoseValidity] DATETIME NOT NULL,
    [TotalTDUReward]     INT      NOT NULL,
    [Redeemed]           BIT      NOT NULL,
    [DateRedeemed]       DATETIME NULL,
    [Paid]               BIT      NOT NULL,
    [DatePaid]           DATETIME NULL,
    [ValidForQuarters]   INT      NOT NULL,
    PRIMARY KEY CLUSTERED ([TDURewardId] ASC),
    CONSTRAINT [FK_TDUReward_ResourceId] FOREIGN KEY ([resourceId]) REFERENCES [dbo].[Resource] ([ResourceId])
);


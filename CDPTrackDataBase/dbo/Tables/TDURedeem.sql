CREATE TABLE [dbo].[TDURedeem] (
    [TDUReedeemId] INT      IDENTITY (1, 1) NOT NULL,
    [resourceId]   INT      NOT NULL,
    [TDUReward]    INT      NULL,
    [QuarterYear]  INT      NOT NULL,
    [Quarter]      INT      NOT NULL,
    [TDU]          INT      NOT NULL,
    [DateReached]  DATETIME NOT NULL,
    [Redeemed]     BIT      NULL,
    [Paid]         BIT      NULL,
    [DateRedeemed] DATETIME NULL,
    [DatePaid]     DATETIME NULL,
    PRIMARY KEY CLUSTERED ([TDUReedeemId] ASC),
    CONSTRAINT [FK_TDURedeem_ResourceId] FOREIGN KEY ([resourceId]) REFERENCES [dbo].[Resource] ([ResourceId]),
    CONSTRAINT [FK_TDURedeem_TDURewardId] FOREIGN KEY ([TDUReward]) REFERENCES [dbo].[TDUReward] ([TDURewardId])
);


use CDPTrack;
go

CREATE TABLE TDUReward(
	TDURewardId int primary key IDENTITY(1,1),
	resourceId int constraint FK_TDUReward_ResourceId REFERENCES
	Resource(ResourceId) not null,
	StartingQuarter int not null,
	EndingQuarter int not null,
	StartingYear int not null,
	EndingYear int not null,
	DatetoLoseValidity datetime not null,
	TotalTDUReward int not null,
	Redeemed bit not null,
	DateRedeemed datetime,
	Paid bit not null,
	DatePaid datetime,
	ValidForQuarters int not null
)
go

go
CREATE TABLE TDURedeem(
	TDUReedeemId int primary key IDENTITY(1,1),
	resourceId int constraint FK_TDURedeem_ResourceId REFERENCES
	Resource(ResourceId) not null,
	TDUReward int constraint FK_TDURedeem_TDURewardId REFERENCES
	TDUReward(TDURewardId) null,
	QuarterYear int not null,
	Quarter int not null,
	TDU int not null,
	DateReached datetime not null,
	Redeemed bit null,
	Paid bit null,
	DateRedeemed datetime null,
	DatePaid datetime null
)
go
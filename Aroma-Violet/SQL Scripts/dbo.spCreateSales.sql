ALTER PROCEDURE [dbo].[spCreateSales]
AS

declare @CompanyControl uniqueidentifier = '5fc87f17-a772-e511-8279-74e6e244c0d6'
declare @SubscriptionSales uniqueidentifier = 'c017f2bc-3361-e511-80c3-2047477ce07a'

--Get Balanaces
    select 
		ca.ClientID,
		a.AccountId,
		sum(Amount) as balance
	into #Balances
	from finJournal j
	inner join finClientAccount ca on ca.ClientAccountId = j.AccountID
	inner join finAccount a on ca.AccountId = a.AccountId
	where --a.IsSystemAccount = 0
	 a.AccountId = @SubscriptionSales --sales account
	--and j.EffectiveDate <= GETDATE()
	group by ca.ClientID,a.AccountId

--Get clients Products
	SELECT
      [ClientID]
	  ,Period
      ,sum([Price]) as Price
	  ,sum(Quantity) as Quantity
  into #products
  FROM [DebitOrderPeriodDetail]
  where Period = year(Dateadd(d,-7,Getdate())) *100 + month(Dateadd(d,-7,Getdate()))
  group by ClientID,Period

--Create sale where money is enought
declare @SystemUserID uniqueidentifier = '00000000-0000-0000-0000-000000000000'


select dod.* 
into #OrderLines
from #products p
inner join #Balances b on p.ClientID = b.ClientID 
left join [DebitOrderPeriodDetail] dod on dod.ClientID = p.ClientID and p.Period = dod.Period
where balance  >= p.Price

select ClientID, newid() OrderHeaderId, sum(Price * Quantity) Total
into #OrderHeaders
from #OrderLines
group by ClientID

begin transaction CreateSale

select 
	distinct
	OrderHeaderId,
	heads.ClientID,
	Total,
	2 OrderStatusID, --Ready to ship
	@SystemUserID UserID,
	getdate() OrderDate,
	1 Active,
	(select top 1 AddressId from [Address] where ClientID = heads.ClientID and AddressTypeID = 2 and Active=1) as AddressID
into #tempHeader
from
	#OrderLines lines
		left join #OrderHeaders heads on  lines.ClientID = heads.ClientID

insert into OrderHeader (OrderHeaderId, ClientID, Total, OrderStatusId, UserId,OrderDate, Active, Address_AddressId,ShippingTypeId)
select
	OrderHeaderId,
	ClientID,
	Total,
	OrderStatusID,
	UserID,
	OrderDate,
	Active,
	AddressID,
	2
from #tempHeader	

insert into OrderLine (OrderHeaderId, ProductID,UnitCost,Quantity,Active)
select 
	heads.OrderHeaderId,
	lines.ProductID,
	lines.Price,
	lines.Quantity,
	1
from
	#OrderLines lines
		left join #OrderHeaders heads on  lines.ClientID = heads.ClientID
	
--create financials

declare @Journals [dbo].[tpfinJournal_GS] 

insert into @Journals
select 
	clientid,
	@SubscriptionSales,
	null,
	@CompanyControl,
	Total,
	GetDate(),
	GetDate(),
	OrderHeaderId,
	'System Generated Sale',
	@SystemUserID
from #tempHeader

--calculate rebates
select 
	c.ClientId, 
	c.ClientTypeID, 
	p1.ClientId ParentId, 
	p1.ClientTypeID ParentClientTypeId, 
	p1.IgnoreRebate ParentIgnoreRebate,
	r1.Amount ParentAmount,
	r1.AccountId ParentAccountId, 
	r1.ProductId,
	p2.ClientId GrandfatherId,
	p2.ClientTypeID GrandfatherClientTypeId,
	p2.IgnoreRebate GrandfatherIgnoreRebate, 
	r2.amount GrandfatherAmount,
	r2.AccountId GrandfatherAccountId,
	OrderHeaderId
into #Rebate
from 
	#tempHeader
	left join client c on #tempHeader.ClientID = c.ClientId
	left join client p1 on c.ResellerID = p1.ClientId
	left join client p2 on p1.ResellerID = p2.ClientId
	left join Rebate r1 on p1.ClientTypeID = r1.ClientTypeId 
	left join Rebate r2 on p2.ClientTypeID = r2.ClientTypeId and r1.ProductId = r2.ProductId
where
	Not r1.ProductId is null
	

	insert into @Journals
	select 
		null,
		@CompanyControl,
		ParentId,
		ParentAccountId,
		ParentAmount,
		GetDate(),
		GetDate(),
		OrderHeaderId,
		'Rebate',
		@SystemUserID
	from #Rebate
	where ParentIgnoreRebate = 0

	insert into @Journals
	select 
		null,
		@CompanyControl,
		GrandfatherId,
		GrandfatherAccountId,
		GrandfatherAmount,
		GetDate(),
		GetDate(),
		OrderHeaderId,
		'Rebate',
		@SystemUserID
	from #Rebate
	where GrandfatherIgnoreRebate = 0

-------------------------------------------------------------------
exec [dbo].[spfinCreateJournal_GS] @Journals

drop table #Rebate
drop table #OrderHeaders
drop table #OrderLines
drop table #Balances
drop table #products

commit transaction CreateSale

RETURN 0
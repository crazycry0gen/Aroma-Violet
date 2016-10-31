-- Get active subscriptions for next 7 days in a temptable where debitorder does not exist yet for period

truncate table DebitOrderPeriodDetail
truncate table finJournal
truncate table DebitOrder
drop table #TempActiveDOClients

select 	
	bd.clientid,
	convert(date,case
		when DAY(CommencementDate) <= DAY(DATEADD(d,2,GETDATE())) then CONVERT(VARCHAR(4),YEAR(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),MONTH(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
		else CONVERT(VARCHAR(4),YEAR(GETDATE())) + '-' + CONVERT(VARCHAR(2),MONTH(GETDATE())) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
	end) as NextDODate,
	YEAR(convert(date,case
		when DAY(CommencementDate) <= DAY(DATEADD(d,2,GETDATE())) then CONVERT(VARCHAR(4),YEAR(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),MONTH(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
		else CONVERT(VARCHAR(4),YEAR(GETDATE())) + '-' + CONVERT(VARCHAR(2),MONTH(GETDATE())) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
	end)) * 100 + MONTH(convert(date,case
		when DAY(CommencementDate) <= DAY(DATEADD(d,2,GETDATE())) then CONVERT(VARCHAR(4),YEAR(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),MONTH(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
		else CONVERT(VARCHAR(4),YEAR(GETDATE())) + '-' + CONVERT(VARCHAR(2),MONTH(GETDATE())) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
	end)) as Period,
	cs.ProductID,
	s.Price
into #TempActiveDOClients
from BankingDetail bd
inner join 
(
	select 
		c.ClientId
	from client c
	inner join ClientSubscription cs on cs.ClientID = c.ClientId	
	where c.Active = 1
	and cs.Active = 1 
	group by C.ClientId
 )clients on clients.ClientId = bd.ClientID 
inner join Client c on c.ClientId = clients.ClientId
inner join ClientSubscription cs on cs.ClientID = c.ClientId
left join Subscription s on s.ProductID = cs.ProductID and s.ClientTypeID = c.ClientTypeID
outer apply 
(	select distinct clientid from DebitOrderPeriodDetail dod 
	where  dod.ClientID = c.ClientId
		and cs.ProductID = dod.ProductID 
		and Period = YEAR(convert(date,case
		when DAY(CommencementDate) <= DAY(DATEADD(d,2,GETDATE())) then CONVERT(VARCHAR(4),YEAR(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),MONTH(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
		else CONVERT(VARCHAR(4),YEAR(GETDATE())) + '-' + CONVERT(VARCHAR(2),MONTH(GETDATE())) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
	end)) * 100 + MONTH(convert(date,case
		when DAY(CommencementDate) <= DAY(DATEADD(d,2,GETDATE())) then CONVERT(VARCHAR(4),YEAR(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),MONTH(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
		else CONVERT(VARCHAR(4),YEAR(GETDATE())) + '-' + CONVERT(VARCHAR(2),MONTH(GETDATE())) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
	end))
) as tempdod
where c.Active = 1
and cs.Active = 1 
and s.Active = 1
and bd.Active = 1 
and
	convert(date,case
		when DAY(CommencementDate) <= DAY(DATEADD(d,2,GETDATE())) then CONVERT(VARCHAR(4),YEAR(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),MONTH(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
		else CONVERT(VARCHAR(4),YEAR(GETDATE())) + '-' + CONVERT(VARCHAR(2),MONTH(GETDATE())) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
	end) < dateadd(d,7,GETDATE())
and convert(date,case
		when DAY(CommencementDate) <= DAY(DATEADD(d,2,GETDATE())) then CONVERT(VARCHAR(4),YEAR(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),MONTH(DATEADD(m,1,GETDATE()))) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
		else CONVERT(VARCHAR(4),YEAR(GETDATE())) + '-' + CONVERT(VARCHAR(2),MONTH(GETDATE())) + '-' + CONVERT(VARCHAR(2),DAY(CommencementDate))
	end) > dateadd(d,2,GETDATE())
and tempdod.clientid is null

insert into DebitOrderPeriodDetail
select 
	ClientId,
	Period,
	ProductID,
	Price,
	1 as DOPStatus -- new
from #TempActiveDOClients


declare @Journals [dbo].[tpfinJournal] 

insert into @Journals
select 
	clientid,
	'6FBDCE30-A772-E511-8279-74E6E244C0D6',
	'22aad92c-ee78-e511-827b-582c80139263',
	sum(price),
	GetDate(),
	DateADD(day,7,NextDODate),
	'Debit Order Run',
	'Debit Order Run',
	'94e870fc-134d-4a7d-8191-3e7acb20c723'
from #TempActiveDOClients
group by ClientId,NextDODate

update DebitOrderPeriodDetail set DebitOrderPeriodStatusID = 2
from #TempActiveDOClients doc
inner join DebitOrderPeriodDetail dod on dod.ClientID = doc.ClientID and dod.ProductID = doc.ProductID and doc.Period = dod.Period 
where DebitOrderPeriodStatusID = 1

exec [dbo].[spfinCreateJournal] @Journals

-- settle DO from commision

declare @MonthEndRun uniqueidentifier = newID();



--Create DO
insert into DebitOrder
(      [ClientID]
      ,[AccountHolderID]
      ,[AccountHolderOtherDetail]
      ,[Initials]
      ,[Surname]
      ,[AccountTypeID]
      ,[DebitDate]
      ,[AccountNumber]
      ,[BankID]
      ,[BranchID]
      ,[SourceID]
      ,[Active]
      ,[Created]
      ,[ProcessDate]	 
	  ,[Amount]
)
select  
	   bd.[ClientID]
      ,[AccountHolderID]
      ,[AccountHolderOtherDetail]
      ,[Initials]
      ,[Surname]
      ,[AccountTypeID]
      ,convert(date,dbo.GetValidDebitOrderDateForCurrentMonth(DAY(NextDODate),MONTH(NextDODate)))
      ,[AccountNumber]
      ,[BankID]
      ,[BranchID]
      ,@MonthEndRun
      ,bc.[Active]
      ,GETDATE()
      ,GETDATE()
	  ,balance
from
(
	select 
		ca.[ClientID],
		sum(Amount) as balance,
		NextDODate
	from finJournal j
	inner join finClientAccount ca on ca.ClientAccountId = j.AccountID
	inner join finAccount a on ca.Account_AccountId = a.AccountId
	inner join (select distinct clientid,NextDODate from #TempActiveDOClients) as temp on temp.ClientID = ca.ClientID 
	where a.IsSystemAccount = 0
	and a.AccountId = '6FBDCE30-A772-E511-8279-74E6E244C0D6'
	and j.EffectiveDate <= GETDATE()
	--and dbo.GetValidDebitOrderDateForCurrentMonth(DAY(NextDODate),MONTH(NextDODate)) > Dateadd(day,2,GETDATE())
	group by ca.ClientID,a.AccountId,NextDODate
) as bd inner join BankingDetail bc on bc.ClientID = bd.clientid

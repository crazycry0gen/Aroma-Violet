drop table #Balances
drop table #products
--Get Balanaces
    select 
		ca.ClientID,
		a.AccountId,
		sum(Amount) as balance
	into #Balances
	from finJournal j
	inner join finClientAccount ca on ca.ClientAccountId = j.AccountID
	inner join finAccount a on ca.Account_AccountId = a.AccountId
	where a.IsSystemAccount = 0
	and a.AccountId = '22aad92c-ee78-e511-827b-582c80139263' --sales account
	--and j.EffectiveDate <= GETDATE()
	group by ca.ClientID,a.AccountId

--Get clients Products
	SELECT
      [ClientID]
	  ,Period
      ,sum([Price]) as Price
  into #products
  FROM [Novus].[dbo].[DebitOrderPeriodDetail]
  where Period = year(Dateadd(d,-7,Getdate())) *100 + month(Dateadd(d,-7,Getdate()))
  group by ClientID,Period

--Create sale where money is enought
select dod.* from #products p
inner join #Balances b on p.ClientID = b.ClientID 
left join [DebitOrderPeriodDetail] dod on dod.ClientID = p.ClientID and p.Period = dod.Period
where balance  >= p.Price
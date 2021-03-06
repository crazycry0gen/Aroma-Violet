ALTER PROCEDURE [dbo].[spfinCreateJournal_GS]
	@journalInput [dbo].[tpfinJournal_GS] READONLY
AS
	DECLARE @bypassData [dbo].[tpfinJournal_Base]
	DECLARE @rowCount int = (select count(1) from @journalInput)
	/*Add trans key*/
	select 
		newid() transId,
		FromClientID,
		FromAccountID,
		ToClientID,
		ToAccountID,
		Amount,
		FromEffectiveDate,
		ToEffectiveDate,
		MovementSource,
		Comment,
		UserID
	into #transInput  
	from @journalInput inp

	set @rowCount = (select count(1) from #transInput)

	select distinct
		transId,
		inp.FromClientID FromClientID,
		FromAccountID,
		inp.ToClientID ToClientID,
		ToAccountID,
		(select top 1 ClientAccountID from finClientAccount where ClientID = inp.FromClientID and AccountId = inp.FromAccountID) ClientFromAccountID,
		(select top 1 ClientAccountID from finClientAccount where ClientID = inp.ToClientID and AccountId = inp.ToAccountID) ClientToAccountID,
		0  CreateFrom,
		0  CreateTo,
		Amount,
		FromEffectiveDate,
		ToEffectiveDate,
		MovementSource MovementSourceID,
		0 CreateNote,
		Comment,
		inp.UserID UserID,
		getdate() JournalDate
	into #tableInput
	from #transInput inp
			
	set @rowCount = (select count(1) from #tableInput)

	update #tableInput set ClientFromAccountID = FromAccountID where FromClientID is null
	update #tableInput set ClientToAccountID = ToAccountID where ToClientID is null

	update #tableInput set ClientFromAccountID = newid(), CreateFrom = 1 where ClientFromAccountID is null
	update #tableInput set ClientToAccountID = newid(), CreateTo = 1 where ClientToAccountID is null


	begin transaction createJournalSupportData
	

	insert into finClientAccount (
		AccountId,
		Active,
		ClientAccountId,
		ClientID
	)
	select 
		fromAccountID,
		1,
		ClientFromAccountID,
		FromClientID
	from
		#tableInput
	where
		#tableInput.CreateFrom = 1

	UNION ALL

	select 
		ToAccountID,
		1,
		ClientToAccountID,
		ToClientID
	from
		#tableInput
	where
		#tableInput.CreateTo = 1

	insert into @bypassData (
		Amount,
		Comment,
		FromAccountID,
		FromEffectiveDate,
		JournalDate,
		MovementSource,
		ToAccountID,
		ToEffectiveDate,
		UserID
		)
	select
		Amount,
		Comment,
		ClientFromAccountID,
		FromEffectiveDate,
		JournalDate,
		MovementSourceID,
		ClientToAccountID,
		ToEffectiveDate,
		UserID
	from #tableInput

	commit transaction createJournalSupportData

	set @rowCount = (select count(1) from @bypassData)

	exec dbo.spfinCreateJournal_Base @bypassData

	

RETURN 0
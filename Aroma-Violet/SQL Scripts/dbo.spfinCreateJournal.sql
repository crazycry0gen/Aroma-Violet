CREATE PROCEDURE [dbo].[spfinCreateJournal]
	@journalInput [dbo].[tpfinJournal] READONLY
AS
	DECLARE @bypassData [dbo].[tpfinJournal_Base]
	DECLARE @rowCount int = (select count(1) from @journalInput)
	/*Add trans key*/
	select 
		newid() transId,
		ClientID,
		FromAccountID,
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
		inp.ClientID ClientID,
		FromAccountID,
		ToAccountID,
		(select top 1 ClientAccountID from finClientAccount where ClientID = inp.ClientID and Account_AccountId = inp.FromAccountID) ClientFromAccountID,
		(select top 1 ClientAccountID from finClientAccount where ClientID = inp.ClientID and Account_AccountId = inp.ToAccountID) ClientToAccountID,
		0  CreateFrom,
		0  CreateTo,
		/*isnull(fromAccount.ClientAccountID,newid()) ClientFromAccountID,
		isnull(toAccount.ClientAccountID,newid()) ClientToAccountID,
		case when fromAccount.ClientAccountID is null then 1 else 0 end CreateFrom,
		case when toAccount.ClientAccountID is null then 1 else 0 end CreateTo,*/
		Amount,
		FromEffectiveDate,
		ToEffectiveDate,
		MovementSource,
		(select top 1 NoteId from SystemNote where NoteText = inp.MovementSource) MovementSourceID,
		0 CreateNote,
		/*isnull(SystemNote.NoteId, newid()) MovementSourceID,
		case when SystemNote.NoteId is null then 1 else 0 end CreateNote,*/
		Comment,
		inp.UserID UserID,
		getdate() JournalDate
	into #tableInput
	from #transInput inp
			/*left join finClientAccount fromAccount
				on inp.ClientID = fromAccount.ClientID 
				and inp.FromAccountID = fromAccount.Account_AccountId
			left join finClientAccount toAccount
				on inp.ClientID = toAccount.ClientID 
				and inp.FromAccountID = toAccount.Account_AccountId
			left join SystemNote 
				on inp.MovementSource = SystemNote.NoteText*/

	set @rowCount = (select count(1) from #tableInput)

	update #tableInput set ClientFromAccountID = newid(), CreateFrom = 1 where ClientFromAccountID is null
	update #tableInput set ClientToAccountID = newid(), CreateTo = 1 where ClientToAccountID is null
	update #tableInput set MovementSourceID = newid(), CreateNote = 1 where MovementSourceID is null


	begin transaction createJournalSupportData
	
	insert into SystemNote (
		NoteId,
		NoteText,
		UserID,
		Created
	)
	select 
		MovementSourceID,
		MovementSource,
		UserID,
		getdate()
	from #tableInput
	where CreateNote = 1

	insert into finClientAccount (
		Account_AccountId,
		Active,
		ClientAccountId,
		ClientID
	)
	select 
		fromAccountID,
		1,
		ClientFromAccountID,
		ClientID
	from
		#tableInput
	where
		#tableInput.CreateFrom = 1

	UNION ALL

	select 
		ToAccountID,
		1,
		ClientToAccountID,
		ClientID
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
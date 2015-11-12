﻿
CREATE PROCEDURE [dbo].spfinCreateJournal_Base
(@journalInput [dbo].[tpfinJournal_Base] READONLY )
AS
	/*Add id's to row data*/
	select 
		FromAccountID,
		ToAccountID,
		Amount,
		FromEffectiveDate,
		ToEffectiveDate,
		JournalDate,
		MovementSource,
		UserID,
		Comment,
		newid() FromEntryID,
		newid() ToEntryID
	into #ntableInput
	from @journalInput

	begin transaction createEntries
	begin
		insert into dbo.finJournal (
		AccountID,
		Active,
		Amount,
		CorrespondingJournalId,
		EffectiveDate,
		JournalDate,
		JournalId,
		MovementSource,
		UserID,
		Comment			
		)
		select
			FromAccountID,
			1,
			Amount,
			ToEntryID,
			FromEffectiveDate,
			JournalDate,
			FromEntryID,
			MovementSource,
			UserID,
			Comment
		from
			#ntableInput

	insert into dbo.finJournal (
		AccountID,
		Active,
		Amount,
		CorrespondingJournalId,
		EffectiveDate,
		JournalDate,
		JournalId,
		MovementSource,
		UserID,
		Comment			
		)
		select
			ToAccountID,
			1,
			Amount,
			FromEntryID,
			ToEffectiveDate,
			JournalDate,
			ToEntryID,
			MovementSource,
			UserID,
			Comment
		from
			#ntableInput
	end
	commit transaction createEntries

	select 
		dbo.finJournal.AccountID,
		dbo.finJournal.Active,
		dbo.finJournal.Amount,
		dbo.finJournal.CorrespondingJournalId,
		dbo.finJournal.EffectiveDate,
		dbo.finJournal.JournalDate,
		dbo.finJournal.JournalId,
		dbo.finJournal.MovementSource,
		dbo.finJournal.UserID
		
	from dbo.finJournal
RETURN 0
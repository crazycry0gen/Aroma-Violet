

ALTER PROCEDURE [dbo].[spCreateNotes]
	@Notes tpNotes readonly,
	@userId uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	declare @dt datetime = getdate()

	select 
		distinct
		sn.NoteId,
		n.NoteText,
		@userId UserId,
		@dt Created
	into #Notes
	from @Notes n
		left join SystemNote sn on n.NoteText = sn.NoteText


	insert into SystemNote (NoteId, NoteText, UserID, Created)
	select distinct newid(), NoteText, UserId, @dt from #Notes where NoteId is null 
	
	update #Notes
	set #Notes.NoteId = sn.NoteId
	from @Notes n
		left join SystemNote sn on n.NoteText = sn.NoteText

	insert into SystemLink (LinkId, Child, Parent, UserID, Created)
	select 
		newid(),
		sn.NoteId,
		ParentId,
		UserId,
		@dt
	from @Notes n
		left join SystemNote sn on n.NoteText = sn.NoteText



END



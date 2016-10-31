CREATE PROCEDURE dbo.spReverseSale
	@OrderIds [dbo].[tpUniqueIdList] readonly,
	@UserId uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;
	------------------------------------------
	--drop table #OrdersToCancel
	--declare @UserId uniqueidentifier = '00000000-0000-0000-0000-000000000000'
	--declare @OrderIds [dbo].[tpUniqueIdList]
	--insert into @OrderIds (id) 
	--select OrderHeaderId from OrderHeader
	------------------------------------------
	
	declare @notes tpNotes

	select 
		o.OrderHeaderId,
		OrderStatusId,
		o.Total,
		o.ClientID
	into #OrdersToCancel	
	from OrderHeader o
			left join OrderLine ol on o.OrderHeaderId = ol.OrderHeaderId
	where o.OrderHeaderId in (select id from @OrderIds) 
		and o.OrderStatusId in (1,2) --only cancel orders that are 'New' or 'ready to Ship'

	insert into @notes (ParentId, NoteText)
	select 
		o.OrderHeaderId,
		'An attempt to cancel the order was made. The status was set to ' + os.OrderStatusName NoteText
	from OrderHeader o
		left join OrderStatus os on o.OrderStatusId = os.OrderStatusId
	where o.OrderHeaderId in (select id from @OrderIds)

	exec dbo.spCreateNotes @notes, @UserId

	begin transaction cancelOrder

	--create journals
	declare @SubscriptionSales uniqueidentifier = 'c017f2bc-3361-e511-80c3-2047477ce07a'
	declare @DebitOrder uniqueidentifier = '6fbdce30-a772-e511-8279-74e6e244c0d6'
	declare @journals tpfinJournal_GS

	insert into @Journals
	select 
		ClientID,
		@SubscriptionSales,
		ClientID,
		@DebitOrder,
		Total,
		getdate(),
		getdate(),
		OrderHeaderId,
		'Order Canceled',
		@UserId
	from
		#OrdersToCancel

	exec dbo.spfinCreateJournal_GS @Journals
	--cansel order
	
	update OrderHeader
	set OrderStatusId=5
	where OrderHeaderId in (select OrderHeaderId from #OrdersToCancel)
	

	--select * from OrderStatus
	--select * from SystemNote

	commit transaction cancelOrder

END
GO

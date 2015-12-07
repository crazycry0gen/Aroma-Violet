ALTER PROCEDURE [dbo].[spBalanceAtDate]
	@date datetime,
	@accountId uniqueidentifier
AS

	declare @balance  money=0, @futureBalance money, @futureDate datetime

	select @balance = sum(Amount) from finJournal where AccountId=@accountId and EffectiveDate <= @date and Active = 1
	select @futureBalance = sum(Amount) from finJournal where AccountId=@accountId  and Active = 1
	select @futureDate = max(EffectiveDate) from finJournal where AccountId=@accountId  and Active = 1

	if(@balance is null) set @balance=0
	if(@futureBalance is null) set @futureBalance=0
	if(@futureDate is null) set @futureDate=getdate()

	select @balance Balance, @futureBalance FutureBalance, @futureDate FutureDate

RETURN 0

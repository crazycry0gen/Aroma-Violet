CREATE FUNCTION [dbo].[GetValidDebitOrderDateForCurrentMonth] 
(
	@Day int
	,@Month int
)
RETURNS date
AS
BEGIN
			
	DECLARE @ReturnDate date
	DECLARE @TempDate date
	--,@Day int = 30
	--,@Month int= 8
		
    SET @TempDate = DATEADD(d, (DAY(GETDATE()) * -1), DATEADD(m,@Month+1,DATEADD(m,MONTH(GETDATE())*-1,GETDATE())));
   -- select @TempDate

	IF (@Day > DAY(@TempDate))
		BEGIN
			SET @Day = DAY(@TempDate);
		END
	
	DECLARE @CheckDate datetime
	SET @CheckDate = CONVERT(datetime, CONVERT(varchar(2), @Day) + '/' + CONVERT(varchar(2),MONTH(@TempDate)) + '/' + CONVERT(varchar(4),YEAR(@TempDate)), 103);
	
	declare @WeekDayNumber int
    set @WeekDayNumber = datepart(weekday, @CheckDate)
    
	WHILE (@WeekDayNumber = 1 or @CheckDate in (select HolidayDate from PublicHolidays with (nolock)))
	BEGIN
		set @CheckDate = DATEADD(DAY,-1,@CheckDate);
		set @WeekDayNumber = datepart(weekday, @CheckDate)
	END	

	-- this is a check if the last day of the month fall on a Saterday that it does not move the debit order into the previous month 
	if @WeekDayNumber = 7 and Month(@CheckDate) <> Month(GETDATE())
	begin
		WHILE (@WeekDayNumber in (1,7) or @CheckDate in (select HolidayDate from PublicHolidays with (nolock)))
		BEGIN
			set @CheckDate = DATEADD(DAY,1,@CheckDate);
			set @WeekDayNumber = datepart(weekday, @CheckDate)
		END	
	end

	SET @ReturnDate = @CheckDate
	
	return  @ReturnDate;
END

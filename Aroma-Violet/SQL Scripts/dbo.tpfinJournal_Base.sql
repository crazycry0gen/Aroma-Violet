CREATE TYPE [dbo].[tpfinJournal_Base] AS TABLE (
    [FromAccountID]     UNIQUEIDENTIFIER NULL,
    [ToAccountID]       UNIQUEIDENTIFIER NULL,
    [Amount]            MONEY            NULL,
    [FromEffectiveDate] DATETIME         NULL,
    [ToEffectiveDate]   DATETIME         NULL,
    [JournalDate]       DATETIME         NULL,
    [MovementSource]    UNIQUEIDENTIFIER NULL,
    [Comment]           VARCHAR (MAX)    NULL,
    [UserID]            UNIQUEIDENTIFIER NULL);


CREATE TYPE [dbo].[tpfinJournal] AS TABLE (
    [ClientID]          INT              NULL,
    [FromAccountID]     UNIQUEIDENTIFIER NULL,
    [ToAccountID]       UNIQUEIDENTIFIER NULL,
    [Amount]            MONEY            NULL,
    [FromEffectiveDate] DATETIME         NULL,
    [ToEffectiveDate]   DATETIME         NULL,
    [MovementSource]    VARCHAR (MAX)    NULL,
    [Comment]           VARCHAR (MAX)    NULL,
    [UserID]            UNIQUEIDENTIFIER NULL);


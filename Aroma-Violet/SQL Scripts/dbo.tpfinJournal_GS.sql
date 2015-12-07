
/****** Object:  UserDefinedTableType [dbo].[tpfinJournal]    Script Date: 11/24/2015 3:49:44 PM ******/
CREATE TYPE [dbo].[tpfinJournal_GS] AS TABLE(
	[FromClientID] [int] NULL,
	[FromAccountID] [uniqueidentifier] NULL,
	[ToClientID] [int] NULL,
	[ToAccountID] [uniqueidentifier] NULL,
	[Amount] [money] NULL,
	[FromEffectiveDate] [datetime] NULL,
	[ToEffectiveDate] [datetime] NULL,
	[MovementSource] [uniqueidentifier] NULL,
	[Comment] [varchar](max) NULL,
	[UserID] [uniqueidentifier] NULL
)
GO



CREATE TABLE [dbo].[SystemSetting] (
    [SettingId]    INT            IDENTITY (1, 1) NOT NULL,
    [SettingKey]   NVARCHAR (MAX) NULL,
    [SettingValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.SystemSetting] PRIMARY KEY CLUSTERED ([SettingId] ASC)
);
GO

CREATE TABLE [dbo].[lgActivityLog] (
    [ActivityLogId] UNIQUEIDENTIFIER DEFAULT (newsequentialid()) NOT NULL,
    [ActivityId]    INT              NOT NULL,
    [IntId]         INT              NULL,
    [GuidId]        UNIQUEIDENTIFIER NULL,
    [Note]          NVARCHAR (MAX)   NULL,
    [iDate]         DATETIME         NOT NULL,
    CONSTRAINT [PK_dbo.lgActivityLog] PRIMARY KEY CLUSTERED ([ActivityLogId] ASC)
);
GO

CREATE TABLE [dbo].[lgActivity] (
    [ActivityLogId] INT            IDENTITY (1, 1) NOT NULL,
    [Activity]      NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_dbo.lgActivity] PRIMARY KEY CLUSTERED ([ActivityLogId] ASC)
);
GO


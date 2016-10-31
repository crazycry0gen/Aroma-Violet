
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/28/2010 14:54:23
-- Generated from EDMX file: C:\Code\CorpSMS\Production\SourceCode\Data\CorpSMS.DataAccess\CorpSMS.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CorpSMS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MessageMessageDetail]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MessageDetail] DROP CONSTRAINT [FK_MessageMessageDetail];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[SendSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SendSetting];
GO
IF OBJECT_ID(N'[dbo].[SentSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SentSetting];
GO
IF OBJECT_ID(N'[dbo].[Message]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Message];
GO
IF OBJECT_ID(N'[dbo].[MessageDetail]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MessageDetail];
GO
IF OBJECT_ID(N'[dbo].[ReplySetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ReplySetting];
GO
IF OBJECT_ID(N'[dbo].[ShortCodeSetting]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ShortCodeSetting];
GO
IF OBJECT_ID(N'[dbo].[Account]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Account];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'SendSetting'
CREATE TABLE [dbo].[SendSetting] (
    [SendSettingID] int IDENTITY(1,1) NOT NULL,
    [IsLive] bit  NOT NULL,
    [ReturnCredits] bit  NOT NULL,
    [ReturnMessageSuccessCount] bit  NOT NULL,
    [ReturnMessageFailedCount] bit  NOT NULL,
    [ReturnEntriesSuccessStatus] bit  NOT NULL,
    [ReturnEntriesFailedStatus] bit  NOT NULL,
    [DefaultSender] nvarchar(max)  NOT NULL,
    [DefaultText1] nvarchar(max)  NOT NULL,
    [DefaultText2] nvarchar(max)  NOT NULL,
    [DefaultDateFormat] nvarchar(max)  NOT NULL,
    [DefaultTimeFormat] nvarchar(max)  NOT NULL,
    [Flash] bit  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [CostCentre] nvarchar(max)  NOT NULL,
    [IsActive] bit  NOT NULL,
    [CreatedBy] nvarchar(max)  NOT NULL,
    [DateCreated] datetime  NOT NULL
);
GO

-- Creating table 'SentSetting'
CREATE TABLE [dbo].[SentSetting] (
    [SentSettingID] int IDENTITY(1,1) NOT NULL,
    [LatestID] bigint  NOT NULL,
    [RecordCount] int  NOT NULL,
    [ReturnColumns] nvarchar(max)  NOT NULL,
    [DateFormat] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Message'
CREATE TABLE [dbo].[Message] (
    [MessageID] int IDENTITY(1,1) NOT NULL,
    [Number] nvarchar(max)  NOT NULL,
    [Sender] nvarchar(max)  NULL,
    [Text] nvarchar(max)  NULL,
    [Created] datetime  NOT NULL
);
GO

-- Creating table 'MessageDetail'
CREATE TABLE [dbo].[MessageDetail] (
    [MessageDetailID] int IDENTITY(1,1) NOT NULL,
    [MessageID] int  NOT NULL,
    [ChangeID] int  NULL,
    [SentID] int  NULL,
    [EventID] int  NULL,
    [Status] nvarchar(max)  NOT NULL,
    [StatusDate] datetime  NOT NULL,
    [Reason] nvarchar(max)  NULL
);
GO

-- Creating table 'ReplySetting'
CREATE TABLE [dbo].[ReplySetting] (
    [ReplySettingID] int IDENTITY(1,1) NOT NULL,
    [LatestID] bigint  NOT NULL,
    [RecordCount] int  NOT NULL,
    [ReturnColumns] nvarchar(max)  NOT NULL,
    [DateFormat] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ShortCodeSetting'
CREATE TABLE [dbo].[ShortCodeSetting] (
    [ShortCodeSettingID] int IDENTITY(1,1) NOT NULL,
    [LatestID] bigint  NOT NULL,
    [DateFormat] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Account'
CREATE TABLE [dbo].[Account] (
    [AccountID] int IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [CountryCodePrefix] nvarchar(max)  NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [SendSettingID] in table 'SendSetting'
ALTER TABLE [dbo].[SendSetting]
ADD CONSTRAINT [PK_SendSetting]
    PRIMARY KEY CLUSTERED ([SendSettingID] ASC);
GO

-- Creating primary key on [SentSettingID] in table 'SentSetting'
ALTER TABLE [dbo].[SentSetting]
ADD CONSTRAINT [PK_SentSetting]
    PRIMARY KEY CLUSTERED ([SentSettingID] ASC);
GO

-- Creating primary key on [MessageID] in table 'Message'
ALTER TABLE [dbo].[Message]
ADD CONSTRAINT [PK_Message]
    PRIMARY KEY CLUSTERED ([MessageID] ASC);
GO

-- Creating primary key on [MessageDetailID] in table 'MessageDetail'
ALTER TABLE [dbo].[MessageDetail]
ADD CONSTRAINT [PK_MessageDetail]
    PRIMARY KEY CLUSTERED ([MessageDetailID] ASC);
GO

-- Creating primary key on [ReplySettingID] in table 'ReplySetting'
ALTER TABLE [dbo].[ReplySetting]
ADD CONSTRAINT [PK_ReplySetting]
    PRIMARY KEY CLUSTERED ([ReplySettingID] ASC);
GO

-- Creating primary key on [ShortCodeSettingID] in table 'ShortCodeSetting'
ALTER TABLE [dbo].[ShortCodeSetting]
ADD CONSTRAINT [PK_ShortCodeSetting]
    PRIMARY KEY CLUSTERED ([ShortCodeSettingID] ASC);
GO

-- Creating primary key on [AccountID] in table 'Account'
ALTER TABLE [dbo].[Account]
ADD CONSTRAINT [PK_Account]
    PRIMARY KEY CLUSTERED ([AccountID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [MessageID] in table 'MessageDetail'
ALTER TABLE [dbo].[MessageDetail]
ADD CONSTRAINT [FK_MessageMessageDetail]
    FOREIGN KEY ([MessageID])
    REFERENCES [dbo].[Message]
        ([MessageID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MessageMessageDetail'
CREATE INDEX [IX_FK_MessageMessageDetail]
ON [dbo].[MessageDetail]
    ([MessageID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
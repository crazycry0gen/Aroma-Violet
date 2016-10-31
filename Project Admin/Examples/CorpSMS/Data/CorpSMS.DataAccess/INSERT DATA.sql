USE [CorpSMS]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ReplySetting_Account]') AND parent_object_id = OBJECT_ID(N'[dbo].[ReplySetting]'))
ALTER TABLE [dbo].[ReplySetting] DROP CONSTRAINT [FK_ReplySetting_Account]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SentSetting_Account]') AND parent_object_id = OBJECT_ID(N'[dbo].[SentSetting]'))
ALTER TABLE [dbo].[SentSetting] DROP CONSTRAINT [FK_SentSetting_Account]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_ShortCodeSetting_Account]') AND parent_object_id = OBJECT_ID(N'[dbo].[ShortCodeSetting]'))
ALTER TABLE [dbo].[ShortCodeSetting] DROP CONSTRAINT [FK_ShortCodeSetting_Account]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Batch_Account]') AND parent_object_id = OBJECT_ID(N'[dbo].[Batch]'))
ALTER TABLE [dbo].[Batch] DROP CONSTRAINT [FK_Batch_Account]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageDetail_Batch]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageDetail]'))
ALTER TABLE [dbo].[MessageDetail] DROP CONSTRAINT [FK_MessageDetail_Batch]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MessageMessageDetail]') AND parent_object_id = OBJECT_ID(N'[dbo].[MessageDetail]'))
ALTER TABLE [dbo].[MessageDetail] DROP CONSTRAINT [FK_MessageMessageDetail]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Credit_Account]') AND parent_object_id = OBJECT_ID(N'[dbo].[Credit]'))
ALTER TABLE [dbo].[Credit] DROP CONSTRAINT [FK_Credit_Account]

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Message_Origin]') AND parent_object_id = OBJECT_ID(N'[dbo].[Message]'))
ALTER TABLE [dbo].[Message] DROP CONSTRAINT [FK_Message_Origin]
GO



GO

TRUNCATE TABLE dbo.[SendSetting]

TRUNCATE TABLE dbo.[SentSetting]

TRUNCATE TABLE dbo.[ReplySetting]

TRUNCATE TABLE dbo.[ShortCodeSetting]

TRUNCATE TABLE dbo.[Account]

TRUNCATE TABLE dbo.[Batch]

TRUNCATE TABLE dbo.[MessageDetail]

TRUNCATE TABLE dbo.[Message]

TRUNCATE TABLE dbo.[Credit]

TRUNCATE TABLE dbo.[Origin]

GO

SET NOCOUNT ON

INSERT INTO Origin(SystemName) SELECT	'Bid Bonanza'
INSERT INTO Origin(SystemName) SELECT	'SMS Portal'

INSERT INTO dbo.[SendSetting] (IsLive, ReturnCredits, ReturnMessageSuccessCount, ReturnMessageFailedCount,
		ReturnEntriesSuccessStatus, ReturnEntriesFailedStatus, DefaultSender, DefaultText1, DefaultText2, 
		DefaultDateFormat, DefaultTimeFormat, Flash, [Type], CostCentre, IsActive, CreatedBy, DateCreated)
SELECT 0, 1, 1, 1,
		1, 1, '', '', '',
		'dd/MMM/yyy', 'HH:mm', 0, 'SMS', 'NA', 1, 'Herman vd Merwe', GETDATE()

DECLARE @accountID INT

INSERT INTO dbo.[Account] ([Description], CountryCodePrefix, Username, [Password], IsDefault)
SELECT 'South Africa',	'27', 'stratSA', 'corp', 1
SET @accountID = SCOPE_IDENTITY()
INSERT INTO dbo.[SentSetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 304415068, 100, 'sentid,eventid,smstype,customerid,status,statusdate', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()
INSERT INTO dbo.[ReplySetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 7788037, 100, 'numfrom,receiveddata,received,sentdatetime,sentcustomerid', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()
INSERT INTO dbo.[ShortCodeSetting](AccountID, LatestID, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 2632653, 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()

INSERT INTO dbo.[Account] ([Description], CountryCodePrefix, Username, [Password])
SELECT 'Namibia', '264', 'stratNamib', 'corp'
SET @accountID = SCOPE_IDENTITY()
INSERT INTO dbo.[SentSetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 304409786, 25, 'sentid,eventid,smstype,customerid,status,statusdate', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()
INSERT INTO dbo.[ReplySetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 7757059, 25, 'numfrom,receiveddata,received,sentdatetime,sentcustomerid', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()

INSERT INTO dbo.[Account] ([Description], CountryCodePrefix, Username, [Password])
SELECT 'Botswana', '267', 'stratBot', 'corp'
SET @accountID = SCOPE_IDENTITY()
INSERT INTO dbo.[SentSetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 304396304, 25, 'sentid,eventid,smstype,customerid,status,statusdate', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()
INSERT INTO dbo.[ReplySetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 0, 25, 'numfrom,receiveddata,received,sentdatetime,sentcustomerid', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()

INSERT INTO dbo.[Account] ([Description], CountryCodePrefix, Username, [Password])
SELECT 'Zimbabwe', '263', 'stratZim', 'corp'
SET @accountID = SCOPE_IDENTITY()
INSERT INTO dbo.[SentSetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 303439943, 25, 'sentid,eventid,smstype,customerid,status,statusdate', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()
INSERT INTO dbo.[ReplySetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 0, 25, 'numfrom,receiveddata,received,sentdatetime,sentcustomerid', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()

INSERT INTO dbo.[Account] ([Description], CountryCodePrefix, Username, [Password])
SELECT 'Lesotho', '266', 'stratLes', 'corp'
SET @accountID = SCOPE_IDENTITY()
INSERT INTO dbo.[SentSetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 304414772, 25, 'sentid,eventid,smstype,customerid,status,statusdate', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()
INSERT INTO dbo.[ReplySetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 7642608, 25, 'numfrom,receiveddata,received,sentdatetime,sentcustomerid', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()

INSERT INTO dbo.[Account] ([Description], CountryCodePrefix, Username, [Password])
SELECT 'Swaziland', '268', 'stratSwazi', 'corp'
SET @accountID = SCOPE_IDENTITY()
INSERT INTO dbo.[SentSetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 304339236, 25, 'sentid,eventid,smstype,customerid,status,statusdate', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()
INSERT INTO dbo.[ReplySetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 7706107, 25, 'numfrom,receiveddata,received,sentdatetime,sentcustomerid', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()

INSERT INTO dbo.[Account] ([Description], CountryCodePrefix, Username, [Password])
SELECT 'Kenya', '254', 'stratken', 'corp'
SET @accountID = SCOPE_IDENTITY()
INSERT INTO dbo.[SentSetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 304371776, 25, 'sentid,eventid,smstype,customerid,status,statusdate', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()
INSERT INTO dbo.[ReplySetting](AccountID, LatestID, RecordCount, ReturnColumns, [DateFormat], IsActive, CreatedBy, DateCreated)
SELECT @accountID, 7777942, 25, 'numfrom,receiveddata,received,sentdatetime,sentcustomerid', 'yyyy/MM/dd HH:mm:ss', 1, USER_NAME(), GETDATE()

GO

ALTER TABLE [dbo].[ReplySetting]  WITH CHECK ADD  CONSTRAINT [FK_ReplySetting_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
ALTER TABLE [dbo].[ReplySetting] CHECK CONSTRAINT [FK_ReplySetting_Account]

ALTER TABLE [dbo].[SentSetting]  WITH CHECK ADD  CONSTRAINT [FK_SentSetting_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
ALTER TABLE [dbo].[SentSetting] CHECK CONSTRAINT [FK_SentSetting_Account]

ALTER TABLE [dbo].[ShortCodeSetting]  WITH CHECK ADD  CONSTRAINT [FK_ShortCodeSetting_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
ALTER TABLE [dbo].[ShortCodeSetting] CHECK CONSTRAINT [FK_ShortCodeSetting_Account]

ALTER TABLE [dbo].[Batch]  WITH CHECK ADD  CONSTRAINT [FK_Batch_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
ALTER TABLE [dbo].[Batch] CHECK CONSTRAINT [FK_Batch_Account]

ALTER TABLE [dbo].[MessageDetail]  WITH CHECK ADD  CONSTRAINT [FK_MessageDetail_Batch] FOREIGN KEY([BatchID])
REFERENCES [dbo].[Batch] ([BatchID])
ALTER TABLE [dbo].[MessageDetail] CHECK CONSTRAINT [FK_MessageDetail_Batch]

ALTER TABLE [dbo].[MessageDetail]  WITH CHECK ADD  CONSTRAINT [FK_MessageMessageDetail] FOREIGN KEY([MessageID])
REFERENCES [dbo].[Message] ([MessageID])
ALTER TABLE [dbo].[MessageDetail] CHECK CONSTRAINT [FK_MessageMessageDetail]

ALTER TABLE [dbo].[Credit]  WITH CHECK ADD  CONSTRAINT [FK_Credit_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([AccountID])
ALTER TABLE [dbo].[Credit] CHECK CONSTRAINT [FK_Credit_Account]

ALTER TABLE [dbo].[Message]  WITH CHECK ADD  CONSTRAINT [FK_Message_Origin] FOREIGN KEY([OriginID])
REFERENCES [dbo].[Origin] ([OriginID])
ALTER TABLE [dbo].[Message] CHECK CONSTRAINT [FK_Message_Origin]
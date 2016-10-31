using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using StratCorp.CorpSMS.DataAccess;
using StratCorp.CorpSMS.Entities;
using StratCorp.CorpSMS.Infrastructure.Helpers;
using System.Diagnostics;

namespace StratCorp.CorpSMS.Logic
{   
    public static class MessageFacade
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void SendSMS(ICollection<MessageEntity> messages)
        {

            List<SendServiceCallEntity> accountList = new List<SendServiceCallEntity>();

            // Validate all messages first
            ValidateMessages(messages);

            SaveInvalidMessages(messages);

            // Get a list of accounts and add messages to these accounts
            accountList = GetAccountList(messages);

            // Get the default account and add all batches.
            accountList.AddRange(GetDefaultAccount(messages));


            foreach (var account in accountList)
            {
                DataSet dataSet = null;
                DataTable entriesTable = null;

                try
                {
                    using (var service = new MyMobileAPIService.APISoapClient())
                    {
                        dataSet = CreateSendSettings(account);
                        entriesTable = CreateEntriesTable();

                        foreach (var entry in account.Entries)
                        {
                            entriesTable.Rows.Add(GetEntryDataRow(entriesTable.NewRow(), entry));
                        }

                        dataSet.Tables.Add(entriesTable);

                        SaveXmlStringToFile(dataSet.GetXml(),"SEND_QUEUE");

                        // Send SMSs via web service.
                        DataSet resultSet = service.Send_DS_DS(account.Account.Username, account.Account.Password, dataSet);

                        dataSet = null;
                        entriesTable = null;

                        SaveXmlStringToFile(resultSet.GetXml(), "SEND_RESULTS");

                        if (resultSet != null && resultSet.Tables.Contains("call_result"))
                        {
                            int eventId = 0;

                            DataTable callResult = resultSet.Tables["call_result"];

                            if (resultSet.Tables.Contains("send_info") && resultSet.Tables["send_info"].Rows.Count > 0)
                            {
                                DataTable sendInfo = resultSet.Tables["send_info"];
                                eventId = TypeSafety.GetValue<int>(sendInfo.Rows[0]["eventid"]);
                            }

                            if (resultSet.Tables.Contains("entries_success") && resultSet.Tables["entries_success"].Rows.Count > 0)
                            {
                                var resultTable = resultSet.Tables["entries_success"];
                                foreach (DataRow row in resultTable.Rows)
                                {
                                    Guid rowGuid;
                                    if (Guid.TryParse(row["customerid"].ToString(), out rowGuid))
                                    {
                                        MessageEntity tempMessage = account.Entries.Where(p => p.RowGuid == rowGuid).FirstOrDefault();
                                        tempMessage.EventId = eventId;
                                        tempMessage.Status = "SENT";
                                        tempMessage.DateCreated = DateTime.Now;

                                        SaveMessage(tempMessage);
                                    }
                                }
                                resultTable = null;
                            }
                            if (resultSet.Tables.Contains("entries_failed") && resultSet.Tables["entries_failed"].Rows.Count > 0)
                            {
                                var resultTable = resultSet.Tables["entries_failed"];
                                foreach (DataRow row in resultTable.Rows)
                                {
                                    Guid rowGuid;
                                    if (Guid.TryParse(row["customerid"].ToString(), out rowGuid))
                                    {
                                        MessageEntity tempMessage = account.Entries.Where(p => p.RowGuid == rowGuid).FirstOrDefault();
                                        tempMessage.EventId = eventId;
                                        tempMessage.Status = "FAILED";
                                        tempMessage.DateCreated = DateTime.Now;

                                        // Create report first, because saving message clears the ValidationReason.
                                        DeliveryReportEntity report = new DeliveryReportEntity()
                                        {
                                            ChangeId = 0,
                                            SentId = 0,
                                            Status = TypeSafety.GetValue<string>(row["reason"]),
                                            StatusDate = DateTime.Now
                                        };

                                        SaveMessage(tempMessage, report);
                                    }
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (entriesTable != null)
                    {
                        entriesTable.Dispose();
                    }
                    if (dataSet != null)
                    {
                        dataSet.Dispose();
                    }
                }
            }
        }

        public static void SendQueuedMessages(Guid batchId)
        {
                using (var dataBinding = new MessageDataAccessor())
                {
                    List<MessageEntity> list = new List<MessageEntity>();

                    foreach (var message in dataBinding.GetQueuedMessages(batchId))
                    {
                        list.Add(new MessageEntity()
                            {
                                NumberTo = message.Number,
                                Text = message.Text,
                                OriginId = message.OriginId,
                                UniqueId = message.UniqueId
                            });
                    }

                    SendSMS(list);

                    // if sent successfully we remove the item from the queue          
                    foreach (MessageEntity message in list)
                    {
                        dataBinding.RemoveSentMessages(message.UniqueId);
                    }
                }
        }

        public static void SendAllQueuedMessages()
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                dataBinding.RemoveBarredMessages();

                List<MessageEntity> list = new List<MessageEntity>();

                ICollection<MessageQueueEntity> queueList = dataBinding.GetTopQueuedMessages();

                while (queueList.Count > 0)
                {
                    list.Clear();
                    foreach (var message in queueList)
                    {
                        list.Add(new MessageEntity()
                        {
                            NumberTo = message.Number,
                            Text = message.Text,
                            OriginId = message.OriginId,
                            UniqueId = message.UniqueId
                        });                        
                    }
                    
                    // send the list of 1000
                    SendSMS(list);

                    
                    // if sent successfully we remove the item from the queue          
                    foreach (MessageEntity message in list)
                    {
                        dataBinding.RemoveSentMessages(message.UniqueId);
                    }

                    queueList = dataBinding.GetTopQueuedMessages();
                }           
            }

        }

        private static DataRow GetEntryDataRow(DataRow entryRow, MessageEntity entry)
        {
            entry.RowGuid = Guid.NewGuid();

            entryRow["numto"] = entry.NumberTo;
            entryRow["customerid"] = entry.RowGuid;
            entryRow["data1"] = entry.Text;

            return entryRow;
        }

        private static void SaveMessage(MessageEntity message)
        {
            SaveMessage(message, null);
        }

        private static void SaveMessage(MessageEntity message, DeliveryReportEntity deliveryReport)
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                message = dataBinding.SetMessage(message);

                if (deliveryReport != null && message != null)
                {
                    deliveryReport.MessageId = message.MessageId;

                    dataBinding.InsertDeliveryReport(deliveryReport);
                }
            }
        }

        private static DataTable CreateEntriesTable()
        {
            // To stop CA2000 from firing, we do all the method calls in try/finally block.
            DataTable tempTable = null;
            DataTable entriesTable = null;

            try
            {
                tempTable = new DataTable("entries");
                tempTable.Columns.Add("numto"); // REQUIRED
                tempTable.Columns.Add("customerid"); // REQUIRED
                tempTable.Columns.Add("data1"); // OPTIONAL (will assume default_data1 if not present)
                entriesTable = tempTable;
                tempTable = null;
            }
            finally
            {
                // If tempTable is not null, an operation has failed and table is disposed to make sure resources are released.
                if (tempTable != null)
                {
                    tempTable.Dispose();
                }
            }

            return entriesTable;
        }

        private static DataSet CreateSendSettings(SendServiceCallEntity account)
        {
            DataSet tempDataSet = null;
            DataTable tempDataTable = null;
            DataSet dataSet = null;

            try
            {
                tempDataSet = new DataSet("senddata");
                tempDataTable = new DataTable("settings");

                tempDataTable.Columns.Add("live"); // OPTIONAL - True/False (default - true)
                tempDataTable.Columns.Add("return_credits"); // OPTIONAL - True/False
                tempDataTable.Columns.Add("return_msgs_success_count"); // OPTIONAL - True/False (default - false)
                tempDataTable.Columns.Add("return_msgs_failed_count"); // OPTIONAL - True/False (default - false)
                tempDataTable.Columns.Add("return_entries_success_status"); // OPTIONAL - True/False (default - false)
                tempDataTable.Columns.Add("return_entries_failed_status"); // OPTIONAL - True/False (default - false)
                tempDataTable.Columns.Add("default_senderid"); // OPTIONAL - 11 Char alphanumeric or 15 char numeric (default - Repliable)
                tempDataTable.Columns.Add("default_date"); // REQUIRED - dd/MMM/yyyy
                tempDataTable.Columns.Add("default_time"); // REQUIRED - HH:mm
                tempDataTable.Columns.Add("default_data1"); // OPTIONAL - ""
                tempDataTable.Columns.Add("default_data2"); // OPTIONAL - ""
                tempDataTable.Columns.Add("default_flash"); // OPTIONAL - True/False (default - false)
                tempDataTable.Columns.Add("default_type"); // OPTIONAL - SMS / WPUSH / VCARD / PORT (default - SMS)
                tempDataTable.Columns.Add("default_costcentre"); // OPTIONAL - ""

                DataRow mainRow = tempDataTable.NewRow();
                mainRow["live"] = account.Settings.IsLive;
                mainRow["return_credits"] = account.Settings.ReturnCredits;
                mainRow["return_msgs_success_count"] = account.Settings.ReturnMessageSuccessCount;
                mainRow["return_msgs_failed_count"] = account.Settings.ReturnMessageFailedCount;
                mainRow["return_entries_success_status"] = account.Settings.ReturnEntriesSuccessStatus;
                mainRow["return_entries_failed_status"] = account.Settings.ReturnEntriesFailedStatus;
                mainRow["default_senderid"] = account.Settings.DefaultSender;
                mainRow["default_date"] = account.Settings.DefaultDate;
                mainRow["default_time"] = account.Settings.DefaultTime;
                mainRow["default_data1"] = account.Settings.DefaultText1;
                mainRow["default_data2"] = account.Settings.DefaultText2;
                mainRow["default_flash"] = account.Settings.Flash;
                mainRow["default_type"] = account.Settings.Type;
                mainRow["default_costcentre"] = account.Settings.CostCentre;

                tempDataTable.Rows.Add(mainRow);
                tempDataSet.Tables.Add(tempDataTable);

                dataSet = tempDataSet;

                tempDataTable = null;
                tempDataSet = null;
            }
            finally
            {
                if (tempDataTable != null)
                {
                    tempDataTable.Dispose();
                }
                if (tempDataSet != null)
                {
                    tempDataSet.Dispose();
                }
            }

            return dataSet;
        }

        private static void SaveInvalidMessages(ICollection<MessageEntity> messages)
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                messages.Where(p => !p.IsValid).ForAll(message =>
                {
                    // If number is null, make it empty
                    if (string.IsNullOrEmpty(message.NumberTo))
                        message.NumberTo = string.Empty;
                    // If text is null, make it empty
                    if (string.IsNullOrEmpty(message.Text))
                        message.Text = string.Empty;
                    // If origin is 0, change it to 6 (Unkown)
                    if (message.OriginId == 0)
                        message.OriginId = 6;

                    message.RowGuid = Guid.NewGuid();
                    message.Status = "VALIDATION FAILED";
                    message.DateCreated = DateTime.Now;

                    // Create report first, because saving message clears the ValidationReason.
                    DeliveryReportEntity report = new DeliveryReportEntity()
                    {
                        ChangeId = 0,
                        SentId = 0,
                        Status = message.ValidationReason,
                        StatusDate = DateTime.Now
                    };

                    SaveMessage(message, report);
                });
            }
        }

        private static List<SendServiceCallEntity> GetDefaultAccount(ICollection<MessageEntity> messages)
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                List<SendServiceCallEntity> dataList = new List<SendServiceCallEntity>();

                // Get the default account for messages without prefixes.
                if (messages.Count(p => !p.HasPrefix) > 0)
                {
                    var account = dataBinding.GetDefaultAccount();
                    if (account != null)
                    {
                        SendServiceCallEntity data = GetSendBatch(account);

                        int counter = 0;
                        List<MessageEntity> entries = new List<MessageEntity>();
                        foreach (var message in messages.Where(p => p.IsValid && !p.HasPrefix))
                        {
                            ProcessMessagesToServiceCall(dataList, account, ref data, ref counter, ref entries, message);
                        }

                        AddMessagesToServiceCallList(dataList, data, entries);
                    }
                }

                return dataList;
            }
        }

        private static List<SendServiceCallEntity> GetAccountList(ICollection<MessageEntity> messages)
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                List<SendServiceCallEntity> dataList = new List<SendServiceCallEntity>();

                foreach (var account in dataBinding.GetAccounts())
                {
                    // Get default settings from database
                    SendServiceCallEntity data = GetSendBatch(account);

                    int counter = 0;
                    List<MessageEntity> entries = new List<MessageEntity>();
                    // Get all the messages where the number starts with the account's prefix
                    // and where the message is valid.
                    foreach (var message in messages.Where(p => p.IsValid && (p.NumberTo.StartsWith(account.CountryCodePrefix)) || (p.NumberTo.StartsWith("+"+account.CountryCodePrefix))))
                    {
                        ProcessMessagesToServiceCall(dataList, account, ref data, ref counter, ref entries, message);
                    }

                    AddMessagesToServiceCallList(dataList, data, entries);
                }

                return dataList;
            }
        }

        private static void AddMessagesToServiceCallList(List<SendServiceCallEntity> dataList, SendServiceCallEntity data, List<MessageEntity> entries)
        {
            // Only add item to list if it contains messages.
            if (entries.Count() > 0)
            {
                foreach (var entry in entries)
                {
                    data.Entries.Add(entry);
                }
                dataList.Add(data);
            }
        }

        private static void ProcessMessagesToServiceCall(List<SendServiceCallEntity> dataList, AccountEntity account, ref SendServiceCallEntity data, ref int counter, ref List<MessageEntity> entries, MessageEntity message)
        {
            message.HasPrefix = true;
            entries.Add(message);

            if (++counter == 1000)
            {
                AddMessagesToServiceCallList(dataList, data, entries);
                entries = new List<MessageEntity>();
                counter = 0;
                data = GetSendBatch(account);
            }
        }

        private static SendServiceCallEntity GetSendBatch(AccountEntity account)
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                var settings = dataBinding.GetSendSettings();
                if (settings != null)
                {
                    DateTime cutoffTime = TypeSafety.GetValue<DateTime>(ConfigurationManager.AppSettings["CutoffTime"], DateTime.Parse("20:00"));
                    DateTime startTime = TypeSafety.GetValue<DateTime>(ConfigurationManager.AppSettings["StartTime"], DateTime.Parse("08:00"));

                    if (DateTime.Compare(DateTime.Now, cutoffTime) > 0)
                    {
                        settings.DefaultTime = startTime.ToString("HH:mm");
                    }
                }

                SendServiceCallEntity data = new SendServiceCallEntity()
                {
                    Account = account,
                    Settings = settings
                };

                return data;
            }
        }

        private static void ValidateMessages(ICollection<MessageEntity> messages)
        {
            messages.ForAll(e =>
            {
                e.Validate<MessageEntity>();
            });
        }

        private static void SaveXmlStringToFile(string xmlString,string origin)
        {
            /*
            string path = @"C:\CorpSMS\";
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }
            string fileName = path + "SMSPortal_" +origin + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
            if (File.Exists(fileName))
            {
                fileName = path + "SMSPortal_" + origin + DateTime.Now.ToString("yyyyMMddHHmmss") + "1.xml";
            }
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.WriteLine(xmlString);
            }
             */
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void RefreshDeliveryReport()
        {
            using (var dataBinding = new MessageDataAccessor())
            using (var service = new MyMobileAPIService.APISoapClient())
            {
                DataSet dataSet = null;
                DataTable settingsTable = null;

                foreach (var account in dataBinding.GetAccounts())
                {
                    bool recordsProcessed;

                    do
                    {
                        recordsProcessed = false;

                        SentServiceCallEntity data = new SentServiceCallEntity()
                        {
                            Settings = dataBinding.GetSentSettings(account.AccountId)
                        };

                        dataSet = new DataSet("sent");
                        settingsTable = new DataTable("settings");

                        settingsTable.Columns.Add("id");
                        settingsTable.Columns.Add("max_recs");
                        settingsTable.Columns.Add("cols_returned");
                        settingsTable.Columns.Add("date_format");

                        DataRow mainRow = settingsTable.NewRow();
                        mainRow["id"] = data.Settings.LatestId;
                        mainRow["max_recs"] = data.Settings.RecordCount;
                        mainRow["cols_returned"] = data.Settings.ReturnColumns;
                        mainRow["date_format"] = data.Settings.DateFormat;
                        settingsTable.Rows.Add(mainRow);
                        dataSet.Tables.Add(settingsTable);

                        DataSet results = service.Sent_DS_DS(account.Username, account.Password, dataSet);

                        SaveXmlStringToFile(results.GetXml(),"DELIVERIES");

                        if (results != null && results.Tables.Contains("data"))
                        {
                            results.Tables["data"].AsEnumerable().ForAll(r =>
                            {
                                 //var r = results.Tables["data"].Rows[0];
                                Guid tempGuid;
                                MessageEntity message;
                                try
                                {

                                    if (Guid.TryParse(TypeSafety.GetValue<string>(r["customerid"]), out tempGuid))
                                    {
                                        message = dataBinding.GetMessageByRowGuid(tempGuid);
                                        // we only update the values of an existing message
                                        if (message != null)
                                        {
                                            message.NumberTo = TypeSafety.GetValue<string>(r["numto"]);
                                            message.NumberTo = (message.NumberTo.Length > 15) ? message.NumberTo.Substring(0, 15) : message.NumberTo;
                                            message.Text = TypeSafety.GetValue<string>(r["data"]);
                                            message.EventId = TypeSafety.GetValue<int>(r["eventid"]);
                                            message.Status = TypeSafety.GetValue<string>(r["status"]);
                                        }
                                        else
                                        {
                                            message = new MessageEntity()
                                            {
                                                // we create a new message as it does not exist
                                                NumberTo = TypeSafety.GetValue<string>(r["numto"]),
                                                Sender = string.Empty,
                                                Text = TypeSafety.GetValue<string>(r["data"]),
                                                DateCreated = DateTime.Now,
                                                OriginId = 5, // Legacy System
                                                UniqueId = TypeSafety.GetValue<int>(r["customerid"], 0),
                                                EventId = TypeSafety.GetValue<int>(r["eventid"]),
                                                RowGuid = Guid.NewGuid(),
                                                Status = "UNKOWN " + TypeSafety.GetValue<string>(r["status"])
                                            };
                                        }
                                        message = dataBinding.SetMessageForced(message);
                                    }
                                    else
                                    {
                                        message = new MessageEntity()
                                            {
                                                // we create a new message as it does not exist
                                                NumberTo = TypeSafety.GetValue<string>(r["numto"]),
                                                Sender = string.Empty,
                                                Text = TypeSafety.GetValue<string>(r["data"]),
                                                DateCreated = DateTime.Now,
                                                OriginId = 5, // Legacy System
                                                UniqueId = TypeSafety.GetValue<int>(r["customerid"], 0),
                                                EventId = TypeSafety.GetValue<int>(r["eventid"]),
                                                RowGuid = Guid.NewGuid(),
                                                Status = "UNKOWN " + TypeSafety.GetValue<string>(r["status"])
                                            };

                                        message = dataBinding.SetMessageForced(message);
                                    }

                                    if (message != null)
                                    {
                                        DeliveryReportEntity report = new DeliveryReportEntity();
                                        report.ChangeId = TypeSafety.GetValue<long>(r["changeid"]);
                                        report.MessageId = message.MessageId;
                                        report.SentId = TypeSafety.GetValue<long>(r["sentid"].ToString());
                                        report.Status = TypeSafety.GetValue<string>(r["status"]);
                                        report.StatusDate = TypeSafety.GetValue<DateTime>(r["statusdate"], DateTime.Now);

                                        dataBinding.InsertDeliveryReport(report);
                                    }
                                }
                                catch (Exception ex) 
                                {
                                    System.Threading.Thread.Sleep(1);
                                }
                            } );

                            recordsProcessed = results.Tables["data"].Rows.Count > 0;

                            if (recordsProcessed)
                            {
                                long latestID = results.Tables["data"].AsEnumerable().Max(s => TypeSafety.GetValue<long>(s["changeid"]));

                                dataBinding.UpdateSentSettingsLatestID(latestID, account.AccountId);
                            }
                        }
                    } while (recordsProcessed);
                }
            }
        }

        public static ICollection<MessageEntity> GetShortCodeMessages(DateTime startTime, DateTime endTime)
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                return dataBinding.GetShortCodeMessages(startTime, endTime);
            }
        }

        public static ICollection<MessageEntity> GetDeliveryReports()
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                return dataBinding.GetDeliveryReports();
            }
        }

        public static ICollection<MessageEntity> GetDeliveryReportByOriginId(int originId, int uniqueId)
        {
            using (var dataBinding = new MessageDataAccessor())
            {
                return dataBinding.GetDeliveryReportByOriginId(originId, uniqueId);
            }
        }

        public static void RefreshCredits()
        {
            using (var service = new MyMobileAPIService.APISoapClient())
            using (var dataBinding = new MessageDataAccessor())
            {
                foreach (var account in dataBinding.GetAccounts())
                {
                    string resultXml = service.Credits_STR(account.Username, account.Password);

                    CreditResultEntity results = Serializer.DeserializeXml<CreditResultEntity>(resultXml);

                    if (results != null && results.Credits != null)
                    {
                        dataBinding.SetCredit(new CreditEntity()
                            {
                                AccountId = account.AccountId,
                                Amount = results.Credits.CreditAmount
                            });
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static void RefreshReplies()
        {
            using (var dataBinding = new MessageDataAccessor())
            using (var service = new MyMobileAPIService.APISoapClient())
            {
                foreach (var account in dataBinding.GetAccounts())
                {
                    bool recordsProcessed;

                    do
                    {
                        recordsProcessed = false;

                        ReplyServiceCallEntity data = new ReplyServiceCallEntity()
                        {
                            Settings = dataBinding.GetReplySettings(account.AccountId)
                        };

                        DataSet dataSet = new DataSet("reply");
                        DataTable settingsTable = new DataTable("settings");
                        settingsTable.Columns.Add("id");
                        settingsTable.Columns.Add("max_recs");
                        settingsTable.Columns.Add("cols_returned");
                        settingsTable.Columns.Add("date_format");

                        DataRow mainRow = settingsTable.NewRow();
                        mainRow["id"] = data.Settings.LatestId;
                        mainRow["max_recs"] = data.Settings.RecordCount;
                        mainRow["cols_returned"] = data.Settings.ReturnColumns;
                        mainRow["date_format"] = data.Settings.DateFormat;
                        settingsTable.Rows.Add(mainRow);
                        dataSet.Tables.Add(settingsTable);

                        DataSet results = service.Reply_DS_DS(account.Username, account.Password, dataSet);

                        if (results != null && results.Tables.Contains("data"))
                        {
                            results.Tables["data"].AsEnumerable().ForAll(r =>
                            {
                                Guid tempGuid;
                                MessageEntity message;

                                if (Guid.TryParse(TypeSafety.GetValue<string>(r["sentcustomerid"]), out tempGuid))
                                {
                                    message = dataBinding.GetMessageByRowGuid(tempGuid);
                                    if (message != null)
                                    {
                                        message.Text = TypeSafety.GetValue<string>(r["sentdata"]);
                                        message.EventId = TypeSafety.GetValue<int>(r["eventid"]);
                                        message.Status = "REPLY";
                                    }
                                    else
                                    {
                                        message = new MessageEntity()
                                        {
                                            NumberTo = string.Empty,
                                            Sender = TypeSafety.GetValue<string>(r["numfrom"]),
                                            Text = TypeSafety.GetValue<string>(r["sentdata"]),
                                            DateCreated = TypeSafety.GetValue<DateTime>(r["sentdatetime"], DateTime.Now),
                                            OriginId = 5, // Legacy
                                            UniqueId = TypeSafety.GetValue<int>(r["eventid"]),
                                            EventId = TypeSafety.GetValue<int>(r["eventid"]),
                                            RowGuid = Guid.NewGuid(),
                                            Status = "UNKNOWN WITH REPLY"
                                        };

                                    }
                                    message = dataBinding.SetMessageForced(message);
                                }
                                else
                                {
                                    message = new MessageEntity()
                                    {
                                        NumberTo = string.Empty,
                                        Sender = TypeSafety.GetValue<string>(r["numfrom"]),
                                        Text = TypeSafety.GetValue<string>(r["sentdata"]),
                                        DateCreated = TypeSafety.GetValue<DateTime>(r["sentdatetime"], DateTime.Now),
                                        OriginId = 5, // Legacy
                                        UniqueId = TypeSafety.GetValue<int>(r["sentcustomerid"],0),
                                        EventId = TypeSafety.GetValue<int>(r["eventid"]),
                                        RowGuid = Guid.NewGuid(),
                                        Status = "UNKNOWN WITH REPLY"
                                    };

                                    message = dataBinding.SetMessageForced(message);
                                }

                                if (message != null)
                                {
                                    MessageReplyEntity reply = new MessageReplyEntity()
                                    {
                                        ReplyId = TypeSafety.GetValue<int>(r["replyid"]),
                                        MessageId = message.MessageId,
                                        Text = TypeSafety.GetValue<string>(r["receiveddata"]),
                                        DateReceived = TypeSafety.GetValue<DateTime>(r["received"], DateTime.Now)
                                    };

                                    dataBinding.InsertMessageReply(reply);           
                                }
                            });

                            recordsProcessed = results.Tables["data"].Rows.Count > 0;
                            
                            if (recordsProcessed)
                            {
                                int latestID = results.Tables["data"].AsEnumerable().Max(s => TypeSafety.GetValue<int>(s["replyid"]));

                                dataBinding.UpdateReplySettingsLatestID(latestID, account.AccountId);
                                dataBinding.AddStopReplies();
                            }
                        }

                    } while (recordsProcessed);
                }
            }
        }

        public static void RefreshShortCodes()
        {
            MessageDataAccessor dataBinding = new MessageDataAccessor();

            using (var service = new MyMobileAPIService.APISoapClient())
            {
                foreach (var account in dataBinding.GetAccounts())
                {
                    bool recordsProcessed;

                    do
                    {
                        recordsProcessed = false;

                        ShortCodeSettingsEntity settings = dataBinding.GetShortCodeSettings(account.AccountId);
                        if (settings != null)
                        {
                            ShortCodeServiceCallEntity data = new ShortCodeServiceCallEntity()
                                {
                                    Settings = settings
                                };

                            string xmlString = Serializer.SerializeXml<ShortCodeServiceCallEntity>(data);

                            string resultXml = service.ShortCode_Get_STR_STR(account.Username, account.Password, xmlString);

                            SaveXmlStringToFile(resultXml,"SHORTCODES");

                            ShortCodeResultEntity results = Serializer.DeserializeXml<ShortCodeResultEntity>(resultXml);

                            if (results.Results != null)
                            {
                                results.Results.ForAll(m =>
                                {
                                    dataBinding.InsertMessageDetail(m);
                                });

                                int latestID = results.Results.Max(r => r.ChangeId);

                                dataBinding.UpdateShortCodeSettingsLatestID(latestID, account.AccountId);

                                recordsProcessed = results.Results.Length > 0;
                            }
                        }
                    } while (recordsProcessed);
                }
            }
        }
    }
}

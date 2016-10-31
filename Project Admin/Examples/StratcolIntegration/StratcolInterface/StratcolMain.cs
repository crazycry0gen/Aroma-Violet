using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace StratcolInterface
{
    public class StratcolMain
    {
        XMLInspector xml = new XMLInspector();

        //auth
        StratcolAuth.authenticateAccessPortTypeClient client = new StratcolAuth.authenticateAccessPortTypeClient();
        StratcolAuth.authenticateClient authDetails = new StratcolAuth.authenticateClient();
        StratcolAuth.authenticateAccess parameter = new StratcolAuth.authenticateAccess();
        StratcolAuth.returnResult result = new StratcolAuth.returnResult();
        
        //Input
        StratcolInput.addStandardTransPortTypeClient clientInput = new StratcolInput.addStandardTransPortTypeClient();        
        StratcolInput.batchHeader batchHeader = new StratcolInput.batchHeader();
        StratcolInput.authenticateClient authInput = new StratcolInput.authenticateClient();
        StratcolInput.result resultInput = new StratcolInput.result();
        //Output
        StratcolOutput.outputStandardTransPortTypeClient clientOutput = new StratcolOutput.outputStandardTransPortTypeClient();
        StratcolOutput.authenticateClient authOutput = new StratcolOutput.authenticateClient();
        StratcolOutput.trans[] resultOutput = new StratcolOutput.trans[10];
        StratcolOutput.@params param = new StratcolOutput.@params();
        //Recall
        StratcolRecall.recallStandardTransPortTypeClient clientRecall = new StratcolRecall.recallStandardTransPortTypeClient();
        StratcolRecall.authenticateClient authRecall = new StratcolRecall.authenticateClient();
        StratcolRecall.recallTrans recallTrans = new StratcolRecall.recallTrans();
        StratcolRecall.returnResult resultRecall = new StratcolRecall.returnResult();
         //Codes
        StratcolCodes.integratorCodesPortTypeClient clientCodes = new StratcolCodes.integratorCodesPortTypeClient();
        StratcolCodes.authenticateClient authCodes = new StratcolCodes.authenticateClient();
        StratcolCodes.inputList paramInput = new StratcolCodes.inputList();
        StratcolCodes.outputDesc[] resultCodes;
        //AHV
        StratcolAhv.outputAhvTransPortTypeClient clientAhv = new StratcolAhv.outputAhvTransPortTypeClient();
        StratcolAhv.authenticateClient authAhv = new StratcolAhv.authenticateClient();
        StratcolAhv.ahvTransRequest parameterAhv = new StratcolAhv.ahvTransRequest();
        StratcolAhv.ahvTranDetails[] resultAhv;
        //Rejected
        StratcolRejected.rejectedStandardTransPortTypeClient clientRejected = new StratcolRejected.rejectedStandardTransPortTypeClient();
        StratcolRejected.authenticateClient authRejected = new StratcolRejected.authenticateClient();
        StratcolRejected.outputRequest parameterRejected = new StratcolRejected.outputRequest();
        StratcolRejected.tranDetails[] resultRejected;        
        //StratcolSearchCanncelled
        StratcolSearchCanncelled.searchCancelledTransPortTypeClient clientSearchCancelled = new StratcolSearchCanncelled.searchCancelledTransPortTypeClient();
        StratcolSearchCanncelled.authenticateClient authSearchCancelled = new StratcolSearchCanncelled.authenticateClient();
        StratcolSearchCanncelled.searchCancelledTrans parameterSearchCancelled = new StratcolSearchCanncelled.searchCancelledTrans();
        StratcolSearchCanncelled.tran[] transactionsSearchCancelled;

        private System.Diagnostics.EventLog eventLog = new System.Diagnostics.EventLog();

        public void StratcolMainInit()
        {         
            if (!System.Diagnostics.EventLog.SourceExists("StratcolIntegration"))
            {
                System.Diagnostics.EventLog.CreateEventSource("StratcolIntegration", "Application");
            }
            eventLog.Source = "StratcolIntegration";
            eventLog.Log = "Application";

            clientRejected.Endpoint.Behaviors.Add(xml);
            clientAhv.Endpoint.Behaviors.Add(xml);
            clientOutput.Endpoint.Behaviors.Add(xml);
            clientSearchCancelled.Endpoint.Behaviors.Add(xml);
        }


        public void UpdateCodes()
        {
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();

            List<string> errorcodes = new List<string>();
            errorcodes.Add("batch_status_codes");
            errorcodes.Add("upfront_rej_codes");
            errorcodes.Add("tran_status_codes");
            errorcodes.Add("tran_type_codes");
            errorcodes.Add("acb_status_codes");
            errorcodes.Add("acb_eft_codes");
            errorcodes.Add("acb_naedo_codes");
            errorcodes.Add("acb_ccard_codes");
            errorcodes.Add("ahv_status_codes");        
            
            var origin = entity.tblOrigins.FirstOrDefault();

                authCodes.key = origin.AuthKey;
                authCodes.mode = origin.TestMode;
                foreach (var errorcode in errorcodes)
                {
                    paramInput.list_type = errorcode;

                    try
                    {
                        resultCodes = clientCodes.integratorCodes(authCodes, paramInput);
                        foreach (var item in resultCodes)
                        {
                            int i;
                            if (Int32.TryParse(item.code, out i))
                            {
                                if (i > 6000)
                                {
                                    System.Threading.Thread.Sleep(10);
                                }
                            }

                            var result = (from x in entity.tblErrorCodes where x.StratcolErrorID == item.code select x).FirstOrDefault();
                            if (result == null)
                            {

                                StratcolDataAccess.tblErrorCode code = new StratcolDataAccess.tblErrorCode();
                                code.StratcolErrorDescription = item.description;
                                code.StratcolErrorID = item.code;
                                entity.AddTotblErrorCodes(code);
                            }
                        }
                        entity.SaveChanges();

                    }
                    catch (Exception ex)
                    {
                        eventLog.WriteEntry("Codes " + ex.InnerException);
                    }
                    System.Threading.Thread.Sleep(10);
                }
            
            // Potential Codes: 
            /*
           
             */
            /* Only use this to update database
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();
            foreach (var item in resultCodes)
            {
                StratcolDataAccess.tblErrorCode code = new StratcolDataAccess.tblErrorCode();
                code.StratcolErrorDescription = item.description;
                code.StratcolErrorID = item.code;
                entity.AddTotblErrorCodes(code);
            }
            entity.SaveChanges();
             */

        }

        public void UpdateOutputs(bool ThreeMonthCheck)
        {
            // get all batches not older than 90 days
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();
            DateTime startDate =  DateTime.Now.AddDays(-90);
            DateTime rejectDate = DateTime.Now.Date;
            List<StratcolDataAccess.tblBatch> batches;
            if (!ThreeMonthCheck)
                batches = entity.tblBatches.Where(x => x.iDate > startDate && rejectDate > x.iDate && x.BatchProcessID != 4 && x.BatchProcessID != 5 && (x.BatchCount > 1 || x.BatchContainsAHV == 0) && x.BatchID >= 1010).ToList();
            else
                batches = entity.tblBatches.Where(x => x.iDate > startDate && rejectDate > x.iDate && x.BatchProcessID != 5 && (x.BatchCount > 1 || x.BatchContainsAHV == 0)).ToList();
            foreach (var batch in batches)
            {
#if DEBUG                
                System.Diagnostics.Debug.Print(batch.BatchID + "\r\n");
#endif
                var origin = entity.tblOrigins.Where(o => o.OriginID == batch.OriginID).FirstOrDefault();

                authOutput.key = origin.AuthKey;
                authOutput.mode = origin.TestMode;
                param.file_code = batch.FileCode;

                try
                {
                    resultOutput = clientOutput.outputStandardTrans(authOutput, param);                    
                    foreach (var item in resultOutput.ToList())
                    {
                        var existingResult = entity.tblResults.Where(r => r.acb_status == item.acb_status && r.batch_ref == item.batch_ref && r.reference == item.reference).FirstOrDefault();
                        if (existingResult == null)
                        {
                            StratcolDataAccess.tblResult result = new StratcolDataAccess.tblResult();
                            result.BacthID = batch.BatchID;
                            result.user_id = item.user_id;
                            result.acb_reason = item.acb_reason.Substring(0, (item.acb_reason.Length < 150)?item.acb_reason.Length:150);
                            result.acb_reason_code = item.acb_reason_code.Substring(0, (item.acb_reason_code.Length < 50) ? item.acb_reason_code.Length : 50);
                            result.acb_status = item.acb_status.Substring(0, (item.acb_status.Length < 50) ? item.acb_status.Length : 50);
                            result.account_no = item.account_no;
                            result.action_date = item.action_date;
                            result.batch_ref = item.batch_ref;
                            result.dbt_cdt_id = item.dbt_cdt_id;
                            result.tran_type = item.tran_type;
                            result.trans_status = item.tran_status.Substring(0, (item.tran_status.Length < 50) ? item.tran_status.Length : 50);
                            result.status_date = item.status_date;
                            result.action_date = item.action_date;
                            result.account_no = item.account_no;
                            result.name = item.name;
                            result.batch_ref = item.batch_ref;
                            result.reference = item.reference;
                            result.otid = item.otid;
                            result.value = item.value;
                            result.iDate = DateTime.Now;
                            result.uDate = DateTime.Now;
                            result.UpdateCount = 1;
                            entity.AddTotblResults(result);
                        }
                        else
                        {
                            existingResult.UpdateCount++;
                            existingResult.status_date = item.status_date;
                            existingResult.uDate = DateTime.Now;
                        }                       
                    }
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    // not processed yet   
                    eventLog.WriteEntry("Output: " + batch.FileCode + " " + xml.lastResponseText.Substring(0, xml.lastResponseText.Length > 1000 ? 1000 : xml.lastResponseText.Length));
#if DEBUG
                    System.Diagnostics.Debug.Print("Output: " + batch.FileCode + " " + xml.lastResponseText.Substring(0, xml.lastResponseText.Length > 1000 ? 1000 : xml.lastResponseText.Length));
#endif
                }
                System.Threading.Thread.Sleep(5000);
            }
        }

        public void UpdateInputs()
        {
            // Get DO without BatchID's
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();

            // Create a Batch and assign it
            entity.CreateBatchForAllOrigins();
            // Submit Batch
            var batches = entity.tblBatches.Where(x => x.BatchProcessID == 2);
            foreach (var batch in batches.ToList())
            {
                var DOs = entity.GetDebitOrdersForBatchID(batch.BatchID);
                // Get Auth details for batcch
                var origin = entity.tblOrigins.Where(o => o.OriginID == batch.OriginID).FirstOrDefault();
                authInput.key = origin.AuthKey;
                authInput.mode = origin.TestMode;

                batchHeader.user_id = origin.UserID;
                batchHeader.batch_ref = batch.BatchID.ToString();
                batchHeader.total_trans = batch.BatchCount;
                batchHeader.total_value = batch.BatchValue;
                StratcolInput.trans[] transactions = new StratcolInput.trans[batch.BatchCount];
                int pos = 0;

                foreach (var debit in DOs.ToList())
                {
                    transactions[pos] = new StratcolInput.trans();
                    transactions[pos].stc_ref = "";
                    transactions[pos].user_ref = debit.OriginUniqueID.ToString();
                    transactions[pos].surname = debit.ClientSurname;
                    transactions[pos].initials = debit.ClientInitials;
                    transactions[pos].mobile_no = "";
                    transactions[pos].account_name = debit.ClientSurname;
                    transactions[pos].branch_code = debit.BranchCode;
                    transactions[pos].id_reg_no = debit.ClientIDNumber;
                    transactions[pos].account_no = debit.AccountNumber;
                    transactions[pos].account_type = debit.AccountTypeID;
                    transactions[pos].ccard_ssv = "";
                    transactions[pos].ccard_exp = "";
                    transactions[pos].user_id = origin.UserID;
                    transactions[pos].start_date = debit.DebitOrderDate.ToString("dd.MM.yy"); //(dd.mm.yy)
                    transactions[pos].amount = Convert.ToDouble(Math.Round(debit.DebitOrderAmount,2));
                    switch (debit.DebitOrderTypeID)
                    {
                        case 1 : transactions[pos].tran_type = "o - once off"; break;
                        case 2 : transactions[pos].tran_type = "nad - naedo"; break;
                        case 3 : transactions[pos].tran_type = "ahv - acc. holder verification"; break;
                        case 4 : transactions[pos].tran_type = "pmt - payment"; break;
                        default : transactions[pos].tran_type = "o - once off"; break;                           
                    }                   
                    transactions[pos].day_of_month = debit.DebitOrderDate.Day.ToString();
                    transactions[pos].@continue = "no";
                    switch (debit.DebitOrderTypeID)
                    {
                        case 2 : transactions[pos].no_of_deduct = "3"; 
                            transactions[pos].final_date =  debit.DebitOrderDate.AddDays(3).ToString("dd.MM.yy"); //(dd.mm.yy)
                            break;
                        default : transactions[pos].no_of_deduct = "1"; 
                             transactions[pos].final_date =  debit.DebitOrderDate.ToString("dd.MM.yy"); //(dd.mm.yy)
                            break;                         
                    }                   
                    transactions[pos].escalation_perc = 0;
                    transactions[pos].escalation_month = "";
                    transactions[pos].publication_ref = debit.OriginUniqueID.ToString();
                    transactions[pos].batch_ref = batch.BatchID.ToString();
                    pos++;
                }
                
                resultInput = clientInput.addStandardTrans(batchHeader, authInput, transactions);                              

                if (resultInput != null)
                {
                  StratcolDataAccess.tblBatchResult batchResult = new StratcolDataAccess.tblBatchResult();
                  batchResult.BatchID = batch.BatchID;
                  batchResult.messageField = resultInput.message;
                  batchResult.file_codeField = resultInput.file_code;
                  batchResult.header_totalField = Convert.ToDecimal(resultInput.header_total);
                  batchResult.header_transField = resultInput.header_trans;
                  batchResult.received_totalField = Convert.ToDecimal(resultInput.received_total);
                  batchResult.received_transField = resultInput.received_trans;
                  batchResult.uploaded_successfulField = resultInput.uploaded_successful;
                  batchResult.uploaded_rejectedField = resultInput.uploaded_rejected;
                  batchResult.iDate = DateTime.Now;                  
                  entity.AddTotblBatchResults(batchResult);
                  entity.SaveChanges();

                  if (resultInput.uploaded_successful > 0)
                  {
                      batch.FileCode = resultInput.file_code;
                      batch.BatchProcessID = 3;
                      entity.SaveChanges();
                  }
                }
                System.Threading.Thread.Sleep(10000);
            }
        }

        public void UpdateAhv()
        {
            // get all batches not older than 120 days
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();
            DateTime startDate = DateTime.Now.AddMonths(-4);
            DateTime rejectDate = DateTime.Now.Date;
            var batches = entity.tblBatches.Where(x => x.iDate > startDate && rejectDate > x.iDate && x.BatchProcessID != 4 && x.BatchContainsAHV == 1);
            foreach (var batch in batches.ToList())
            {
                var origin = entity.tblOrigins.Where(o => o.OriginID == batch.OriginID).FirstOrDefault();

                authAhv.key = origin.AuthKey;
                authAhv.mode = origin.TestMode;
                parameterAhv.file_code = batch.FileCode;

                try
                {
                    resultAhv = clientAhv.outputAhvTrans(authAhv, parameterAhv);
                    foreach (var item in resultAhv.ToList())
                    {
                        var existingResult = entity.tblAccountHolderVerifications.Where(r => r.id_no == item.id_no && r.batch_ref == item.batch_ref && r.account_no == item.account_no && r.acb_status == item.acb_status).FirstOrDefault();
                        if (existingResult == null)
                        {
                            StratcolDataAccess.tblAccountHolderVerification result = new StratcolDataAccess.tblAccountHolderVerification();
                            result.user_id = item.user_id;
                            result.trans_status = item.tran_status;
                            result.acb_status = item.acb_status;
                            result.id_no_match = item.id_no_match;
                            result.acc_no_match = item.acc_no_match;
                            result.acc_open = item.acc_open;
                            result.allow_debit = item.allow_debit;
                            result.allow_credit = item.allow_credit;
                            result.open3_months = item.open3_months;
                            result.surname_match = item.surname_match;
                            result.initials_match = item.initials_match;
                            result.otid = item.otid;
                            result.batch_ref = item.batch_ref;
                            result.surname = item.surname;
                            result.initials = item.initials;
                            result.id_no = item.id_no;
                            result.branch_code = item.branch_code;
                            result.account_no = item.account_no;
                            result.status_date = item.status_date;
                            result.iDate = DateTime.Now;
                            result.uDate = DateTime.Now;
                            result.UpdateCount = 1;
                            entity.AddTotblAccountHolderVerifications(result);
                        }
                        else
                        {
                            existingResult.UpdateCount++;
                            existingResult.uDate = DateTime.Now;
                        }      
                    }
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    // not processed yet
                    eventLog.WriteEntry("AHV: " + batch.FileCode + " " + xml.lastResponseText);
                }
            }
        }

        public void UpdateRejected()
        {
            // get all batches not older than 120 days
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();
            DateTime startDate = DateTime.Now.AddDays(-10);
            DateTime rejectDate = DateTime.Now.Date;
            var batches = entity.tblBatches.Where(x => x.iDate > startDate && rejectDate > x.iDate && x.BatchRejectedRequested == 0 && x.BatchProcessID != 5);
            foreach (var batch in batches.ToList())
            {
                var origin = entity.tblOrigins.Where(o => o.OriginID == batch.OriginID).FirstOrDefault();

                authRejected.key = origin.AuthKey;
                authRejected.mode = origin.TestMode;
                parameterRejected.file_code = batch.FileCode;
                
                try
                {                    
                    resultRejected = clientRejected.rejectedStandardTrans(authRejected, parameterRejected);
                    foreach (var item in resultRejected.ToList())
                    {
                        
                        var existingResult = entity.tblResults.Where(r => r.acb_reason_code == item.acb_reason_code && r.batch_ref == item.batch_ref && r.reference == item.reference).FirstOrDefault();
                        if (existingResult == null)
                        {
                            StratcolDataAccess.tblResult result = new StratcolDataAccess.tblResult();
                            result.BacthID = batch.BatchID;
                            result.user_id = item.user_id;
                            result.acb_reason = item.acb_reason.Substring(0, (item.acb_reason.Length < 150) ? item.acb_reason.Length : 150); 
                            result.acb_reason_code = item.acb_reason_code;
                            result.acb_status = item.acb_status;
                            result.account_no = item.account_no;
                            result.action_date = item.action_date;
                            result.batch_ref = item.batch_ref;
                            result.dbt_cdt_id = item.dbt_cdt_id;
                            result.tran_type = item.tran_type;
                            result.trans_status = item.tran_status;
                            result.status_date = item.status_date;
                            result.action_date = item.action_date;
                            result.account_no = item.account_no;
                            result.name = item.name;
                            result.batch_ref = item.batch_ref;
                            result.reference = item.reference;
                            result.otid = item.otid;
                            result.value = item.value;
                            result.uDate = DateTime.Now;
                            result.iDate = DateTime.Now;
                            entity.AddTotblResults(result);
                        }
                        else
                        {
                            existingResult.UpdateCount++;
                            existingResult.uDate = DateTime.Now;
                        }
                        batch.BatchRejectedRequested = 1;
                    }                                       
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    // not processed yet               
                    eventLog.WriteEntry("Rejection: " + batch.FileCode + " " + xml.lastResponseText);
                    if (xml.lastResponseText.Contains("Parameter:No rejected transactions available."))
                    {
                        batch.BatchRejectedRequested = 1;
                        entity.SaveChanges();
                    }
                }
            }
        }

        public void UpdateAuths()
        {
            // get all batches not older than 120 days
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();
            
            foreach (var origin in entity.tblOrigins.ToList())
            {
                authDetails.key = origin.AuthKey;
                authDetails.mode = origin.TestMode;
                parameter.@params = origin.AuthParam;
                try
                {
                    result = client.authenticateAccess(authDetails, parameter);
                    MessageBox.Show(result.message);
                }catch(Exception ex)
                {}
            }
        }

        public void ProcessResults()
        {
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();
            entity.JobProcessDebitOrderResults();
            entity.JobProcessAHVResults();
        }

        public void RecallBatch()
        {
            try
            {
                StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();
                int OriginID = 8;
                var origin = entity.tblOrigins.Where(o => o.OriginID == OriginID).FirstOrDefault();

                authRecall.key = origin.AuthKey;
                authRecall.mode = origin.TestMode;

                recallTrans.file_code = "46MP1503";
                recallTrans.received_total = 88078.15;
                recallTrans.received_trans = 517;
                resultRecall = clientRecall.recallStandardTrans(authRecall, recallTrans);
            }
            catch (Exception ex)
            {
                if (xml.lastResponseText.Contains("Parameter:No rejected transactions available."))
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
        }

        public void SearchCancelled()
        {
            // get all batches not older than 120 days
            StratcolDataAccess.SCGSAStratcolPortalEntities entity = new StratcolDataAccess.SCGSAStratcolPortalEntities();
            DateTime startDate = DateTime.Now.AddDays(-60);
            DateTime cancelledDate = DateTime.Now.Date;
            var batches = entity.tblBatches.Where(x => x.iDate > startDate && cancelledDate > x.iDate && x.BatchValue > 0);
            foreach (var batch in batches.ToList())
            {
                var origin = entity.tblOrigins.Where(o => o.OriginID == batch.OriginID).FirstOrDefault();
            
                authSearchCancelled.key = origin.AuthKey;
                authSearchCancelled.mode = origin.TestMode;
                
                parameterSearchCancelled.account_no = "";
                parameterSearchCancelled.cancel_date_from = "";
                parameterSearchCancelled.cancel_date_to = "";
                parameterSearchCancelled.date_from = "";
                parameterSearchCancelled.date_to = "";
                parameterSearchCancelled.file_code = batch.FileCode;                
                parameterSearchCancelled.batch_ref = batch.BatchID.ToString();
                

                try
                {
                    transactionsSearchCancelled = clientSearchCancelled.searchCancelledTrans(authSearchCancelled, parameterSearchCancelled);
                    foreach (var item in transactionsSearchCancelled.ToList())
                    {

                        var existingResult = entity.tblCancelledDebitOrders.Where(r => r.cancel_reason == item.cancel_reason && r.batch_ref == item.batch_ref && r.reference == item.reference).FirstOrDefault();
                        if (existingResult == null)
                        {
                            StratcolDataAccess.tblCancelledDebitOrder result = new StratcolDataAccess.tblCancelledDebitOrder();
                            result.account_no = item.account_no;
                            result.action_date =  item.action_date;
                            result.batch_ref = item.batch_ref;
                            result.cancel_date =  item.cancel_date;
                            result.cancel_reason = item.cancel_reason;
                            result.dbt_cdt_id = item.dbt_cdt_id;                            
                            result.reference = item.reference;
                            result.name = item.name;
                            result.otid = item.otid;
                            result.tran_type = item.tran_type;
                            result.user_id = item.user_id;
                            result.value = Convert.ToDecimal(item.value);
                            result.idate = DateTime.Now;
                            result.udate = DateTime.Now;
                            result.UpdateCount = 1;
                            entity.AddTotblCancelledDebitOrders(result);                             
                        }
                        else
                        {                                                        
                            existingResult.UpdateCount++;
                            existingResult.udate = DateTime.Now;                                                          
                        }
                    }
                    entity.SaveChanges();
                }
                catch (Exception ex)
                {
                    // not processed yet           
                    if (!xml.lastResponseText.Contains("Parameter:No cancelled transactions found."))
                    {
                        eventLog.WriteEntry("Search Cancellation: Batch" + batch.BatchID + " " + xml.lastResponseText);
                    }
                }
            }
        }
    }
}

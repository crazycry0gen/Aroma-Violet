using System;
using Aroma_Violet.Models;
using System.Collections.Generic;
using System.Linq;

namespace Aroma_Violet.Models
{
    public class ReportAccountBalanceViewModel
    {
        public List<ReportAccountBalanceViewModelDetail> Detail { get; set; }
        public string[,] Matrix { get; internal set; }
        public Guid?[,] Links { get; internal set; }

        public ReportAccountBalanceViewModel()
        {
            Detail = new List<ReportAccountBalanceViewModelDetail>();
        }
    }
    public class ReportAccountBalanceViewModelDetail
    {
        public int ClientId { get; set; }
        public Guid ClientAccountId { get; set; }
        public string AccountName { get; set; }
        public decimal Balance { get; set; }
    }

    public class ReportCommisionStatementViewModel
    {
        public int ClientId { get; internal set; }
        public string Description { get; internal set; }
        public string Name { get; internal set; }
        public List<ReportCommisionStatementPeriodViewModel> Periods { get; internal set; }
    }

    public class ReportLinkedUserSales
    {
        public DateTime ToDate { get; internal set; }

        public ReportLinkedUserSalesViewModel[] ClientSales { get; internal set; }
        public DateTime FromDate { get; internal set; }
    }

    public class ReportLinkedUserSalesViewModel
    {
        public decimal Total { get; internal set; }
        public int UserClientId { get; internal set; }
        public Guid UserId { get; internal set; }
    }

    public class ReportCommisionStatementPeriodViewModel
    {

        public decimal Amount { get; internal set; }
        public decimal AmountPer { get; internal set; }
        public decimal AmountValue { get; internal set; }
        public ReportCommisionStatementPeriodDetailViewModel[] Detail { get; internal set; }
        public int Level { get; internal set; }
        public string PeriodEnd { get; internal set; }
        public string PeriodStart { get; internal set; }
        public decimal SubscriptionFirstProduct { get; internal set; }
        public decimal SubscriptionFirstProductPer { get; internal set; }
        public decimal SubscriptionFirstProductValue { get; internal set; }
        public decimal SubscriptionOtherProduct { get; internal set; }
        public decimal SubscriptionOtherProductPer { get; internal set; }
        public decimal SubscriptionOtherProductValue { get; internal set; }
    }

    public class ReportCommisionStatementPeriodDetailViewModel
    {
        public decimal Amount { get; internal set; }
        public string Cell { get; internal set; }
        public Client Client { get; internal set; }
        public string ClientDescription { get; internal set; }
        public string EMail { get; internal set; }
        public string Introducer { get; internal set; }
        public decimal SubscriptionAmount { get; internal set; }
    }

    public class ReportDownlineStatementViewModel
    {
        public string Name { get; set; }
        public int ClientId { get; set; }
        public List<ReportDownlineStatementResellerViewModel> Resellers { get; set; }
        public List<ReportDownlineStatementSubscriberViewModel> Subscribers { get; set; }
        public List<ReportDownlineStatementSalesValue> SalesValues { get; set; }
        public ReportDownlineStatementViewModel(Client client)
        {
            this.Name = string.Format("{0} {1}", client.ClientInitials, client.ClientSurname);
            this.ClientId = client.ClientId;
        }
    }

    public class ReportDownlineStatementResellerViewModel: ReportDownlineStatementChildViewModel
    {
        public decimal TotalPurchases { get; set; }
        public decimal AveragePurchases { get; set; }
        public ReportDownlineStatementResellerViewModel(Client client, int level, string cell, string email, Client parent)
            :base( client,  level,  cell,  email,  parent)
        {

        }
    }

    public class ReportDownlineStatementSalesValue
    {
        public string MonthDescription { get; set; }
        public decimal Total { get; set; }
        public decimal Average { get; set; }
        public decimal Subscription { get; set; }
    }

    public class ReportDownlineStatementSubscriberViewModel: ReportDownlineStatementChildViewModel
    {
        public decimal SubscriptionAmount { get; set; }
        public ReportDownlineStatementSubscriberViewModel(Client client, int level, string cell, string email, Client parent)
            :base( client,  level,  cell,  email,  parent)
        {

        }
    }
    public abstract class ReportDownlineStatementChildViewModel
    {
        public ReportDownlineStatementChildViewModel(Client client, int level, string cell, string email, Client parent)
        {
            Level = level;
            Name = string.Format("{0} {1}", client.ClientInitials, client.ClientSurname);
            ClientId = client.ClientId;
            ClientType = client.ClientType.ClientTypeName;
            Status = client.Active ? "Active" : "Inactive";
            DateJoined = client.iDate.ToString(Generic.LongDateNoTime);
            CellphoneNo = cell;
            EmailAddress = email;
            if (parent != null)
            {
                IntroducerName = string.Format("{0} {1}", parent.ClientInitials, parent.ClientSurname); ;
                IntroducerClientId = parent.ClientId;
            }
        }
        public int Level { get; set; }
        public string Name { get; set; }
        public int ClientId { get; set; }
        public string ClientType { get; set; }
        public string Status { get; set; }
        public string DateJoined { get; set; }
        public string CellphoneNo { get; set; }
        public string EmailAddress { get; set; }
        public string IntroducerName { get; set; }
        public int IntroducerClientId { get; set; }
    }

    public class ReportViewModel
    {
        public DateTime FromDate { get; internal set; }
        public int[] SalesTypeId { get; internal set; }
        public DateTime ToDate { get; internal set; }
        public static List<ReportViewSaleModel> Sales { get; set; }
        public static List<ReportViewClientModel> Clients { get; set; }
        public static List<ReportViewLineItemModel> Lines { get; set; }
        public string Total { get; internal set; }
        public string Shipping { get; internal set; }
        public string TotalIncShipping { get; internal set; }
        public static List<KeyValuePair<string,string>> PaymentSummary { get;  set; }
        public static List<ReportViewGroupedLineItemModel> GroupedLines { get; internal set; }
    }

    public class ReportViewClientModel
    {
        public int ClientID { get; set; }
        public string Description { get; internal set; }

        public List<ReportViewSaleModel> Sales()
        {
            return (from ReportViewSaleModel item in ReportViewModel.Sales
                    where item.ClientID == this.ClientID
                    select item).ToList();
        }
    }

    public class ReportViewSaleModel
    {
        public Guid? ChildId { get; internal set; }
        public int ClientID { get; set; }
        public string Description { get; internal set; }
        public string Invoice { get; internal set; }
        public Guid OrderHeaderId { get; internal set; }
        public string PaymentType { get; internal set; }
        public string Source { get; internal set; }
        public string Total { get; internal set; }
        public string TotalExcl { get; internal set; }
        public decimal TotalValue { get; internal set; }
        public string Shipping { get; internal set; }
        public decimal ShippingValue { get; internal set; }
        public string TotalIncShipping { get; internal set; }

        public List<ReportViewLineItemModel> Sales()
        {
            return (from ReportViewLineItemModel item in ReportViewModel.Lines
                    where item.ClientID == this.ClientID
                    select item).ToList();
        }
    }

    public class ReportViewLineItemModel
    {
        public string Quantity { get; set; }
        public string UnitPrice { get; set; }

        public int ClientID { get; set; }
        public string ProductCode { get; internal set; }
        public string ProductDescription { get; internal set; }
        public string Total { get; internal set; }
        public decimal TotalValue { get; internal set; }
        public Guid OrderHeaderId { get; internal set; }
        public int QuantityValue { get; internal set; }
    }


    public class ReportViewGroupedLineItemModel
    {
        public int QuantityValue { get; set; }
        public string UnitPrice { get; set; }

        public string ProductCode { get; internal set; }
        public string ProductDescription { get; internal set; }
        public string Total { get; internal set; }
        public decimal TotalValue { get; internal set; }
        public string Quantity { get; internal set; }
        
    }
}
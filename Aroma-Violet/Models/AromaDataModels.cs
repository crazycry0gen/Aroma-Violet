using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Aroma_Violet.Models
{
    public class AromaContext : DbContext
    {
        public AromaContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<AromaContext>(null);
        }

        public DbSet<ClientType> ClientTypes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<UserClient> UserClients { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<EthnicGroup> EthnicGroups { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<IncomeGroup> IncomeGroups { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressLine> AddressLines { get; set; }
        public DbSet<PostalCode> PostalCodes { get; set; }
        public DbSet<PostalArea> PostalAreas { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<BankingDetail> BankingDetails { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<ClientSubscription> ClientSubscriptions { get; set; }
        public DbSet<SystemNote> SystemNotes { get; set; }
        public DbSet<SystemLink> SystemLinks { get; set; }
        public DbSet<finAccount> Accounts { get; set; }
        public DbSet<finClientAccount> ClientAccounts { get; set; }
        public DbSet<finJournal> Journals { get; set; }
        public DbSet<DebitOrder> DebitOrders { get; set; }
        public DbSet<SystemSMS> SystemSMSes { get; set; }
        public DbSet<SupportTicketStatus> SupportTicketStatuses { get; set; }
        public DbSet<SystemSMSStatus> SystemSMSStatuses { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<ClientRelationship> ClientRelationShips { get; set; }
        public DbSet<SystemSMSTemplate> SystemSMSTemplates { get; set; }
        public DbSet<SystemSMSEvent> SystemSMSEvents { get; set; }
        public DbSet<SystemTicketTemplate> SystemTicketTemplates { get; set; }
        public DbSet<ClientGuid> ClientGuids { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<LGActivityLog> ActivityLogs { get; set; }
        public DbSet<LGActivity> Activities { get; set; }
        public DbSet<SystemMenuList> SystemMenuList { get; set; }
        public DbSet<SystemMenuListItem> SystemMenuListItems { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<CreditNote> CreditNotes { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<SystemIntervalSpecifier> SystemIntervalSpecifiers { get; set; }
        public DbSet<SystemProcedure> SystemProcedures { get; set; }
        public DbSet<SystemProcedureMessage> SystemProcedureMessages { get; set; }
        public DbSet<SupportTicketType> SupportTicketTypes { get; set; }
        public DbSet<DebitOrderPeriodDetail> DebitOrderPeriodDetails { get; set; }
        public DbSet<DebitOrderPeriodStatus> DebitOrderPeriodStatuses { get; set; }
        public DbSet<PublicHolidays> PublicHolidays { get; set; }
        public DbSet<Rebate> Rebates { get; set; }
        public DbSet<ShippingType> ShippingTypes { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<PickingListHeader> PickingListHeaders { get; set; }
        public DbSet<PickingListDetail> PickingListDetails { get; set; }
        public DbSet<ShippingMethodPostalCode> ShippingMethodPostalCodes { get; set; }
        public DbSet<LabelPrintedGroup> LabelPrintedGroup { get; set; }
        public DbSet<LevelRange> LevelRanges { get; set; }
        public DbSet<RebateRange> RebateRanges { get; set; }
        public DbSet<SalesTable> SalesTables { get; set; }
        public DbSet<RebateLevelsTable> RebateLevelsTables { get; set; }
        public DbSet<RebateLevelsTableRow> RebateLevelsTableRows { get; set; }
        public DbSet<RebateClientType> RebateClientTypes { get; set; }
        public DbSet<CalculationPeriod> CalculationPeriod { get; set; }
        public DbSet<SubscriptionSaleRebateCalculation> SubscriptionSaleRebateCalculation { get; set; }
        public DbSet<GroupSaleRebateCalculation> GroupSaleRebateCalculation { get; set; }
        public DbSet<GroupSaleRebateCalculationOrderHeader> GroupSaleRebateCalculationOrderHeader { get; set; }
        public DbSet<StoredProcedureSchedule> StoredProcedureSchedules { get; set; }
        public DbSet<RepeatEveryUnit> RepeatEveryUnit { get; set; }
        public DbSet<GroupSaleRebateCalculationSummary> GroupSaleRebateCalculationSummaries { get; set; }
        public DbSet<GroupSaleRebateCalculationRevenue> GroupSaleRebateCalculationRevenues { get; set; }
        public DbSet<GroupRebateCalculationRevenueSummary> GroupRebateCalculationRevenueSummaries { get; set; }
        public DbSet<SalesType> SalesTypes { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<DebitCreditRule> DebitCreditRules { get; set; }
        public DbSet<PostageCharge> PostageCharges { get; set; }
        public DbSet<PostalCodePostageCharge> PostalCodePostageCharges { get; set; }
        public DbSet<MonthEnd> MonthEndRows { get; set; }
        public DbSet<MonthEndDetail> MonthEndDetail { get; set; }
        public DbSet<RebateSalesTableClientType> RebateSalesTableClientTypes { get; set; }
        public DbSet<SystemEvent> SystemEvents { get; set; }
        public DbSet<ClientTypeProductObligation> ClientTypeProductObligations { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Entity<OrderHeader>().Property(x => x.Total).HasPrecision(16, 3);
            modelBuilder.Entity<OrderHeader>().Property(x => x.Shipping).HasPrecision(16, 3);
            modelBuilder.Entity<OrderHeader>().Property(x => x.VAT).HasPrecision(16, 3);
            modelBuilder.Entity<OrderLine>().Property(x => x.UnitCost).HasPrecision(16, 3);
            modelBuilder.Entity<OrderLine>().Property(x => x.UnitCostExcl).HasPrecision(16, 3);
            modelBuilder.Entity<Subscription>().Property(x => x.Price).HasPrecision(16, 3);
            modelBuilder.Entity<Subscription>().Property(x => x.PriceExcl).HasPrecision(16, 3);
        }

        public System.Data.Entity.DbSet<Aroma_Violet.Models.InterAccountTransferViewModel> InterAccountTransferViewModels { get; set; }

        
    }

    
    [Table("ClientTypeProductObligation")]
    public class ClientTypeProductObligation
    {
        [Key]
        public int ClientTypeProductObligationId { get; set; }
        public int ProductId { get; set; }
        public int ClientTypeId { get; set; }
        public bool Active { get; set; }

        [ForeignKey("ProductId")]
        virtual public Product Product { get; set; }

        [ForeignKey("ClientTypeId")]
        virtual public ClientType ClientType { get; set; }
    }

    [Table("SystemEvent")]
    public class SystemEvent
    {
        [Key]
        public Guid SystemEventId { get; set; }

        [DisplayName("Event Name")]
        public string SystemEventName { get; set; }

        [DisplayName("User")]
        public Guid? UserId { get; set; }
    }



        [Table("UserClient")]
    public class UserClient
    {
        [Key]
        public Guid UserId { get; set; }
        
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
    }
        #region New Monthend tables
        [Table("MonthEnd")]
    public class MonthEnd
    {
        [Key]
        public int MonthEndRowId { get; set; }
        public int PeriodId { get; set; }
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }
        public int ClientTypeId { get; set; }
        public int DownlineIndex { get; set; }
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [DataType(DataType.Currency)]
        public decimal SubscriptionFirstProduct { get; set; }
        [DataType(DataType.Currency)]
        public decimal SubscriptionOtherProduct { get; set; }

        public decimal AmountPer { get; set; }
        public decimal AmountValue { get; set; }
        public bool AmountQualify { get; set; }
        public Guid AmountFromAccount { get; set; }
        public Guid AmountToAccount { get; set; }
        public decimal SubscriptionFirstProductPer { get; set; }
        public decimal SubscriptionFirstProductValue { get; set; }
        public decimal SubscriptionOtherProductPer { get; set; }
        public decimal SubscriptionOtherProductValue { get; set; }
        public Guid SubscriptionFromAccount { get; set; }
        public Guid SubscriptionToAccount { get; set; }
        public Guid ApprovedUserId { get; set; }
        public Guid Reference { get; set; }

        public virtual ICollection<MonthEndDetail> Detail { get; set; }
        
    }
    [Table("MonthEndDetail")]
    public class MonthEndDetail
    {
        [Key]
        public int MonthEndDetailId { get; set; }

        public int MonthEndRowId { get; set; }
        [ForeignKey("MonthEndRowId")]
        public virtual MonthEnd MonthEnd { get; set; }

        public Guid OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public virtual OrderHeader OrderHeader {get;set;}
    }
        #endregion

        #region Stored Procedure Scheduling
        [Table("StoredProcedureSchedule")]
    public class StoredProcedureSchedule
    {
        [Key]
        public Guid StoredProcedureScheduleId { get; set; }

        public string StoredProcedureName { get; set; }

        public string StoredProcedureDescription { get; set; }

        public DateTime RunAfter { get; set; }
        public DateTime LastRun { get; set; }
        public DateTime NextRun { get; set; }

        public int RepeatEvery { get; set; }

        public int RepeatEveryUnitId { get; set; }
        [ForeignKey("RepeatEveryUnitId")]
        public RepeatEveryUnit RepeatEveryUnit { get; set; }
    }

    [Table("StoredProcedureScheduleUnit")]
    public class RepeatEveryUnit
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RepeatEveryUnitId { get; set; }

        public string RepeatEveryUnitName { get; set; }

        public int RelatesToMilisecond { get; set; }

    }
    #endregion

    #region Rebates
    [Table("LevelRange")]
    public class LevelRange
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LevelRangeId { get; set; }

        [DisplayName("Start Level")]
        public int StartLevel { get; set; }
        [DisplayName("End Level")]
        public int EndLevel { get; set; }
    }

    [Table("RebateSalesTable")]
    public class SalesTable
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RebateSalesTableId { get; set; }

        public int RebateClientTypeId { get; set; }
        [ForeignKey("RebateClientTypeId")]
        public RebateClientType RebateClientType { get; set; }

        [DisplayName("Description")]
        public string Description { get; set; }

        [DisplayName("Level Range")]
        public int LevelRangeId { get; set; }
        [ForeignKey("LevelRangeId")]
        public virtual LevelRange LevelRange { get; set; }

        public Guid FromAccountId { get; set; }
        [DisplayName("From Account")]
        [ForeignKey("FromAccountId")]
        public virtual finAccount FromAccount { get; set; }

        public Guid ToAccountId { get; set; }
        [DisplayName("To Account")]
        [ForeignKey("ToAccountId")]
        public virtual finAccount ToAccount { get; set; }

        [DisplayName("Rebate Ranges")]
        public virtual ICollection<RebateRange> RebateRanges { get; set; }

        [DataType(DataType.Currency)]
        public decimal MinOwnPurchToQualify { get; set; }
        
        [ForeignKey("RebateSalesTableId")]
        public virtual ICollection<RebateSalesTableClientType> RebateSalesTableClientTypes { get; set; }
    }

    [Table("RebateRange")]
    public class RebateRange
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RebateRangeId { get; set; }

        [DisplayName("Range Start")]
        [DataType(DataType.Currency)]
        public decimal RangeStart { get; set; }

        [DisplayName("Range End")]
        [DataType(DataType.Currency)]
        public decimal RangeEnd { get; set; }

        [DisplayName("% Rebate")]
        public decimal Rebate { get; set; }

        public int RebateSalesTableId { get; set; }

        [DisplayName("Sales Table")]
        [ForeignKey("RebateSalesTableId")]
        public SalesTable RebateSalesTable { get; set; }
    }





    [Table("RebateLevelsTableRow")]
    public class RebateLevelsTableRow
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RebateLevelsTableRowId { get; set; }

        public int RebateLevelsTableId { get; set; }
        [ForeignKey("RebateLevelsTableId")]
        public RebateLevelsTable RebateLevelsTable { get; set; }

        [DisplayName("Level Range")]
        public int LevelRangeId { get; set; }
        public virtual LevelRange LevelRange { get; set; }

        [DisplayName("% First Product")]
        public decimal FirstProductRebate { get; set; }

        [DisplayName("% Additional Products")]
        public decimal AdditionalProductsRebate { get; set; }
    }

    [Table("RebateClientType")]
    public class RebateClientType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RebateClientTypeId { get; set; }

        [Required]
        [DisplayName("Client Type")]
        public int ClientTypeID { get; set; }
        public virtual ClientType ClientType { get; set; }

        public int RebateLevelsTableId { get; set; }
        [DisplayName("Rebate on Levels")]
        [ForeignKey("RebateLevelsTableId")]
        public RebateLevelsTable RebateLevelsTable { get; set; }

        [DisplayName("Sales Tables")]
        public virtual ICollection<SalesTable> SalesTables { get; set; }

    }

    [Table("RebateSalesTableClientType")]
    public class RebateSalesTableClientType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RebateSalesTableClientTypeId { get; set; }

        public int RebateSalesTableId { get; set; }

        public int ClientTypeId { get; set; }

        public bool Active { get; set; }
    }

    [Table("RebateLevelsTable")]
    public class RebateLevelsTable
    {
        [Key, ForeignKey("RebateClientType")]
        /*[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]*/
        public int RebateClientTypeId { get; set; }

        [DisplayName("Rows")]
        public virtual ICollection<RebateLevelsTableRow> RebateLevelRows { get; set; }

        public Guid FromAccountId { get; set; }
        [DisplayName("From Account")]
        [ForeignKey("FromAccountId")]
        public virtual finAccount FromAccount { get; set; }

        public Guid ToAccountId { get; set; }
        [DisplayName("To Account")]
        [ForeignKey("ToAccountId")]
        public virtual finAccount ToAccount { get; set; }

        public RebateClientType RebateClientType { get; set; }

    }
    #endregion

    #region System Control

    [Table("SystemProcedure")]
    public class SystemProcedure
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid SystemProcedureId { get; set; }

        [DisplayName("Name")]
        public string ProcedureName { get; set; }
        [DisplayName("Descriprion")]
        public string Proceduredescription { get; set; }
        [DisplayName("Interval Specifier")]
        public int IntervalSpecifierId { get; set; }
        public virtual SystemIntervalSpecifier IntervalSpecifier { get; set; }
        [DisplayName("Interval")]
        public int Interval { get; set; }
        public bool Active { get; set; }
        [DisplayName("Last Run")]
        public DateTime LastRun { get; set; }
    }

    [Table("LabelPrintedGroup")]
    public class LabelPrintedGroup
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LablePrintedGroupId { get; set; }

        public Guid GroupId { get; set; }
        public Guid BatchId { get; set; }
        public DateTime Printed { get; set; }
    }

    [Table("SystemProcedureMessage")]
    public class SystemProcedureMessage
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid SystemProcedureMessageId { get; set; }

        [DisplayName("System Procedure")]
        public Guid SystemProcedureId { get; set; }
        public virtual SystemProcedure SystemProcedure { get; set; }

        [DisplayName("Message Date")]
        public DateTime MessageDate { get; set; }

        public string Message { get; set; }
    }

    [Table("SystemIntervalSpecifier")]
    public class SystemIntervalSpecifier
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IntervalSpecifierId { get; set; }

        [DisplayName("Name")]
        public string IntervalSpecifierName { get; set; }

        [DisplayName("Milisecond Converter")]
        public int MilisecondConverter { get; set; }
    }
    #endregion

    #region Shipping

    [Table("PostageCharge")]
    public class PostageCharge
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PostageChargeId { get; set; }

        [DisplayName("Name")]
        public string PostageChargeName { get; set; }

        [DisplayName("Charge")]
        [DataType(DataType.Currency)]
        public decimal Charge { get; set; }

        public bool Active { get; set; }
    }

    [Table("PostalCodePostageCharge")]
    public class PostalCodePostageCharge
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PostalCodePostageChargeId { get; set; }


        [ForeignKey("PostageCharge")]
        public int PostageChargeId { get; set; }

        [ForeignKey("PostalCode")]
        public int PostalCodeId { get; set; }

        public virtual PostageCharge PostageCharge { get; set; }
        public virtual PostalCode PostalCode { get; set; }
    }

    [Table("ShippingMethodPostalCode")]
    public class ShippingMethodPostalCode
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ShippingMethodPostalCodeId { get; set; }

        [DisplayName("Postal Code")]
        public int PostalCodeId { get; set; }
        public virtual PostalCode PostalCode { get; set; }

        [DisplayName("Shipping Method")]
        public int ShippingMethodId { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }

        public string Description { get; set; }

        [DisplayName("Extra Cost")]
        [DataType(DataType.Currency)]
        public decimal ExtraCost { get; set; }

        public bool Active { get; set; }

    }

    [Table("PickingListHeader")]
    public class PickingListHeader
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PickingListHeaderId { get; set; }

        [DisplayName("Shipping Type")]
        public int ShippingTypeId { get; set; }
        public virtual ShippingType ShippingType { get; set; }

        [DisplayName("Shipping Method")]
        public int ShippingMethodId { get; set; }
        public virtual ShippingMethod ShippingMethod { get; set; }

        [DisplayName("Picking Date")]
        public DateTime? PickingDate { get; set; }

        [DisplayName("Shipped Date")]
        public DateTime? ShippedDate { get; set; }

        [DisplayName("Detail")]
        public virtual IEnumerable<PickingListDetail> PickingListDetail { get; set; }
    }
    [Table("PickingListDetail")]
    public class PickingListDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PickingListDetailId { get; set; }

        [DisplayName("Picking List Header")]
        public int PickingListHeaderId { get; set; }
        public virtual PickingListHeader PickingListHeader { get; set; }

        [DisplayName("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [DisplayName("Total Items")]
        public int TotalItems { get; set; }

        [DisplayName("Transfer Quantity")]
        public int TransferQuantity { get; set; }
        [DisplayName("Client Code")]

        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [DisplayName("Shiping Address")]
        public virtual Address Address { get; set; }

        [DisplayName("Order")]
        public int OrderLineId { get; set; }
        public virtual OrderLine OrderLine { get; set; }

        [DisplayName("Tracking Number")]
        public string TrackingNumber { get; set; }
        public bool Active { get; set; }

        public Guid GroupId { get; set; }
        public string Invoice { get; internal set; }
    }
    #endregion

    #region Order
    [Table("ShippingMethod")]
    public class ShippingMethod
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ShippingMethodId { get; set; }
        [DisplayName("Shipping Method")]
        public string ShippingMethodName { get; set; }
        [DisplayName("Extra Cost")]
        [DataType(DataType.Currency)]
        public decimal ExtraCost { get; set; }
        public bool Active { get; set; }
    }

    [Table("ShippingType")]
    public class ShippingType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ShippingTypeId { get; set; }
        [DisplayName("Shipping Type")]
        public string ShippingTypeName { get; set; }
        [DisplayName("Show on Picking List")]
        public bool PickingList { get; set; }
        [DisplayName("Show on Shipping List")]
        public bool ShippingList { get; set; }
        public bool Active { get; set; }
    }

    [Table("OrderHeader")]
    public class OrderHeader
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid OrderHeaderId { get; set; }

        [DisplayName("Client Code")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [DisplayName("Total")]
        //[DataType(DataType.Currency)]
        public decimal Total { get; set; }

        [DisplayName("Shipping")]
        //[DataType(DataType.Currency)]
        public decimal Shipping { get; set; }

        [DisplayName("VAT")]
        //[DataType(DataType.Currency)]
        public decimal VAT { get; set; }

        [DisplayName("Order Lines")]
        public virtual ICollection<OrderLine> OrderLines { get; set; }

        [DisplayName("Shiping Address")]
        public virtual Address Address { get; set; }

        [DisplayName("Order Status")]
        public int OrderStatusId { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }

        [DisplayName("User")]
        public Guid UserId { get; set; }

        [DisplayName("Order Date")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Shipping Type")]
        public int ShippingTypeId { get; set; }
        public virtual ShippingType ShippingType { get; set; }

        [DisplayName("Sale Source")]
        public int SaleSourceId { get; set; }

        [DisplayName("Sales Type")]
        public int SalesTypeId { get; set; }

        [ForeignKey("SalesTypeId")]
        public virtual SalesType SalesType { get; set; }
        
        public bool Active { get; set; }

        public bool OnceOff { get; set; }
    }

    [Table("Invoice")]
    public class Invoice
    {
        [Key]
        public Guid InvoiceId { get; set; }

        public string Number { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual OrderHeader OrderHeader { get; set; }

        [DisplayName("Shipping Description")]
        public string ShippingDescription { get; set; }

        [DisplayName("ShippingCharge")]
        [DataType(DataType.Currency)]
        public decimal ShippingCharge { get; set; }
    }

    [Table("CreditNote")]
    public class CreditNote
    {
        [Key]
        public Guid CreditNoteId { get; set; }

        public string Number { get; set; }
        
        [ForeignKey("CreditNoteId")]
        public virtual OrderHeader OrderHeader { get; set; }

        [ForeignKey("CreditNoteId")]
        public virtual Invoice Invoice {get;set;}
    }


    [Table("OrderLine")]
    public class OrderLine
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OrderLineId { get; set; }

        [DisplayName("Order Header")]
        public Guid OrderHeaderId { get; set; }
        public virtual OrderHeader OrderHeader { get; set; }

        [DisplayName("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [DisplayName("Unit Cost")]
        //[DataType(DataType.Currency)]
        public decimal UnitCost { get; set; }

        [DisplayName("Unit Cost")]
        //[DataType(DataType.Currency)]
        public decimal UnitCostExcl { get; set; }

        [DisplayName("Quantity")]
        public int Quantity { get; set; }

        public bool Active { get; set; }

    }


    #endregion

    #region Menu Control
    [Table("SystemMenuListItem")]
    public class SystemMenuListItem
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid SystemMenuListItemId { get; set; }
        public string Text { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Parameters { get; set; }
        public int Order { get; set; }
        public bool Active { get; set; }

    }

    [Table("SystemMenuList")]
    public class SystemMenuList
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SystemMenuListId { get; set; }
        public Guid RoleId { get; set; }
        public Guid SystemMenuListItemId { get; set; }
        public virtual SystemMenuListItem SystemMenuListItem { get; set; }
        public bool Active { get; set; }
    }
    #endregion

    #region Lookup tables


    [Table("PaymentType")]
    public class PaymentType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PaymentTypeId { get; set; }

        [DisplayName("Payment Type")]
        public string PaymentTypeName { get; set; }

        public bool Active { get; set; }
    }

    [Table("SalesType")]
    public class SalesType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SalesTypeId { get; set; }

        [DisplayName("Sales Type")]
        public string SalesTypeDescription { get; set; }

        public bool Active { get; set; }

        [DisplayName("Sale Source Account")]
        public Guid AccountId { get; set; }

        [ForeignKey("AccountId")]
        public finAccount SourceAccount { get; set; }

        [DisplayName("Default Shipping Type")]
        public int DefaultShippingTypeId { get; set; }

        [ForeignKey("DefaultShippingTypeId")]
        public ShippingType DefaultShippingType { get; set; }
    }

    [Table("OrderStatus")]
    public class OrderStatus
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OrderStatusId { get; set; }

        [DisplayName("Order Status")]
        public string OrderStatusName { get; set; }

        public bool Active { get; set; }
    }

    [Table("SupportTicketType")]
    public class SupportTicketType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SupportTickettypeId { get; set; }

        [DisplayName("Support Ticket Type")]
        public string SupportTicketTypeName { get; set; }

    }

    [Table("SupportTicketStatus")]
    public class SupportTicketStatus
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SupportTicketStatusId { get; set; }
        [DisplayName("Name")]
        public string SupportTicketStatusName { get; set; }
        public bool Active { get; set; }

    }

    [Table("SystemSMSStatus")]
    public class SystemSMSStatus
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SystemSMSStatusId { get; set; }
        [DisplayName("Name")]
        public string SystemSMSStatusName { get; set; }
        public bool Active { get; set; }

    }

    [Table("SystemSMSEvent")]
    public class SystemSMSEvent
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SystemSMSEventId { get; set; }
        [DisplayName("Name")]
        public string SystemSMSEventName { get; set; }
        public int SystemSMSTemplateId { get; set; }
        [ForeignKey("SystemSMSTemplateId")]
        public virtual SystemSMSTemplate SystemSMSTemplate { get; set; }
        public bool Active { get; set; }
    }


    [Table("ClientType")]
    public class ClientType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ClientTypeId { get; set; }
        [Required]
        [DisplayName("Client Type")]
        public string ClientTypeName { get; set; }
        public bool AddShipping { get; set; }
        public bool Active { get; set; }

        /*public virtual ICollection<Client> Clients { get; set; }*/
    }

    [Table("Title")]
    public class Title
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TitleId { get; set; }
        [Required]
        [DisplayName("Title")]
        public string TitleName { get; set; }
        public bool Active { get; set; }

    }


    [Table("EthnicGroup")]
    public class EthnicGroup
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EthnicGroupId { get; set; }
        [Required]
        [DisplayName("Ethnic Group")]
        public string EthnicGroupName { get; set; }
        public bool Active { get; set; }

    }


    [Table("Language")]
    public class Language
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LanguageId { get; set; }
        [Required]
        [DisplayName("Language")]
        public string LanguageName { get; set; }
        public bool Active { get; set; }

    }


    [Table("IncomeGroup")]
    public class IncomeGroup
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IncomeGroupId { get; set; }
        [Required]
        [DisplayName("Income")]
        public string IncomeGroupName { get; set; }
        public bool Active { get; set; }

    }

    [Table("AddressType")]
    public class AddressType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AddressTypeId { get; set; }
        [Required]
        [DisplayName("Address Type")]
        public string AddressTypeName { get; set; }
        public bool Active { get; set; }

    }

    [Table("PostalCode")]
    public class PostalCode
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PostalCodeId { get; set; }
        [Required]
        [DisplayName("Postal Code")]
        public string PostalCodeName { get; set; }
        [DisplayName("Postal area")]
        public int PostalAreaId { get; set; }
        [ForeignKey("PostalAreaId")]
        public virtual PostalArea PostalArea { get; set; }
        [DisplayName("Province")]
        public int ProvinceId { get; set; }
        [ForeignKey("ProvinceId")]
        public virtual Province Province { get; set; }
        [DisplayName("Country")]
        public int CountryId { get; set; }
        [ForeignKey("CountryId")]
        public virtual Country Country { get; set; }

        public bool Active { get; set; }

    }

    [Table("PostalArea")]
    public class PostalArea
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int PostalAreaId { get; set; }
        [Required]
        [DisplayName("Postal Area")]
        public string PostalAreaName { get; set; }
        public bool Active { get; set; }

    }

    [Table("Province")]
    public class Province
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProvinceId { get; set; }
        [Required]
        [DisplayName("Province")]
        public string ProvinceName { get; set; }
        public bool Active { get; set; }

    }

    [Table("Country")]
    public class Country
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }
        [Required]
        [DisplayName("Country")]
        public string CountryName { get; set; }
        public bool Active { get; set; }

    }

    [Table("ContactType")]
    public class ContactType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ContactTypeId { get; set; }
        [Required]
        [DisplayName("Contact Type")]
        public string ContactTypeName { get; set; }
        public bool Active { get; set; }

    }

    [Table("AccountHolder")]
    public class AccountHolder
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AccountHolderId { get; set; }
        [Required]
        [DisplayName("AccountHolder")]
        public string AccountHolderName { get; set; }
        public bool Active { get; set; }

    }


    [Table("AccountType")]
    public class AccountType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AccountTypeId { get; set; }
        [Required]
        [DisplayName("Account Type")]
        public string AccountTypeName { get; set; }
        public bool Active { get; set; }

    }


    [Table("Bank")]
    public class Bank
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BankId { get; set; }
        [Required]
        [DisplayName("Bank")]
        public string BankName { get; set; }
        public bool Active { get; set; }

    }

    [Table("Branch")]
    public class Branch
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Branch")]
        public string BranchName { get; set; }
        [Required]
        [DisplayName("Branch Code")]
        public string BranchCode { get; set; }
        public bool Active { get; set; }
        [Required]
        public int BankId { get; set; }

    }
    #endregion

    #region Data object tables


    [Table("SupportTicket")]
    public class SupportTicket
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid SupportTicketId { get; set; }
        public string Description { get; set; }

        [DisplayName("Client Code")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [DisplayName("Type")]
        public int SupportTicketTypeId { get; set; }
        public virtual SupportTicketType SupportTicketType { get; set; }

        [DisplayName("Assigned User")]
        public Guid? UserID { get; set; }

        [DisplayName("Status")]
        public int SupportTicketStatusID { get; set; }
        public virtual SupportTicketStatus SupportTicketStatus { get; set; }

        [DisplayName("Created")]
        public DateTime iDate { get; set; }
    }


    [Table("SystemTicketTemplate")]
    public class SystemTicketTemplate
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SystemQueryTemplateId { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public bool Active { get; set; }
    }

    [Table("SystemSMSTemplate")]
    public class SystemSMSTemplate
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SystemSMSTemplateId { get; set; }
        public string Description { get; set; }
        public string Text { get; set; }
        public bool Active { get; set; }
    }


    [Table("SystemSMS")]
    public class SystemSMS
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SystemSMSId { get; set; }
        [DisplayName("Client Code")]
        public int ClientID { get; set; }
        [DisplayName("Number")]
        public string Number { get; set; }
        [DisplayName("Text")]
        public string SMSDescription { get; set; }
        [DisplayName("Created")]
        public DateTime iDate { get; set; }
        public DateTime? Sent { get; set; }
        [DisplayName("Last Send Attempt")]
        public DateTime? LastSendAttempt { get; set; }
        [DisplayName("Last send Message")]
        public string LastSendMessage { get; set; }
        public bool Active { get; set; }
        public Guid? Source { get; set; }
        [DisplayName("Status")]
        public int SystemSMSStatusId { get; set; }
        public virtual SystemSMSStatus SystemSMSStatus { get; set; }
    }

    [Table("ClientRelationship")]
    public class ClientRelationship
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [DisplayName("Client Relationship ID")]
        public Guid ClientRelationshipId { get; set; }

        [Required]
        [DisplayName("Parent ID")]
        public int ParentID { get; set; }

        [Required]
        [DisplayName("Child ID")]
        public int ChildID { get; set; }

        public bool Active { get; set; }
    }

    [Table("ClientGuid")]
    public class ClientGuid
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [DisplayName("Client Key")]
        public Guid ClientKey { get; set; }
        [DisplayName("Client Code")]
        public int ClientId { get; set; }
    }

    [Table("Client")]
    public class Client
    {
        [Key]
        /*[DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]*/
        [DisplayName("Client Code")]
        public int ClientId { get; set; }

        [Required]
        [DisplayName("Initials")]
        public string ClientInitials { get; set; }

        [DisplayName("Nick Name")]
        public string NickName { get; set; }

        [DisplayName("Full Names")]
        public string FullNames { get; set; }

        [DisplayName("Occupation")]
        public string Occupation { get; set; }

        [Required]
        [DisplayName("Home Language")]
        public int LanguageID { get; set; }
        public virtual Language Language { get; set; }

        [Required]
        [DisplayName("Employer")]
        public string Employer { get; set; }

        [Required]
        [DisplayName("Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [DisplayName("Surname")]
        public string ClientSurname { get; set; }

        [Required]
        [DisplayName("SA Resident")]
        public bool SAResident { get; set; }

        [Required]
        [DisplayName("ID / Passport Number")]
        public string IDNumber { get; set; }

        [Required]
        [DisplayName("Client Type")]
        public int ClientTypeID { get; set; }
        public virtual ClientType ClientType { get; set; }

        [Required]
        [DisplayName("Title")]
        public int TitleID { get; set; }
        public virtual Title Title { get; set; }

        [DisplayName("Ethnic Group")]
        public int EthnicGroupID { get; set; }
        public virtual EthnicGroup EthnicGroup { get; set; }

        [DisplayName("Income Group")]
        public int IncomeGroupID { get; set; }
        public virtual IncomeGroup IncomeGroup { get; set; }

        public int PostalAddress_AddressId { get; set; }
        [DisplayName("Postal Address")]
        [ForeignKey("PostalAddress_AddressId")]
        public virtual Address PostalAddress { get; set; }

        public int DeliveryAddress_AddressId { get; set; }
        [DisplayName("Delivery Address")]
        [ForeignKey("DeliveryAddress_AddressId")]
        public virtual Address DeliveryAddress { get; set; }

        [Required]
        [DisplayName("Province")]
        public int? ProvinceID { get; set; }
        public virtual Province Province { get; set; }

        [Required]
        [DisplayName("Country")]
        public int CountryID { get; set; }
        public virtual Country Country { get; set; }

        [DisplayName("Subscriptions")]
        public virtual ICollection<ClientSubscription> ClientSubscriptions { get; set; }

        [DisplayName("Banking Detail")]
        //public int BankingDetailID { get; set; }
        public virtual ICollection<BankingDetail> BankingDetails { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Registration Number")]
        public string RegistrationNumber { get; set; }

        [DisplayName("Introducer")]
        public int? ResellerID { get; set; }
        
        public bool Active { get; set; }

        public bool IgnoreRebate { get; set; }

        public DateTime iDate { get; set; }

        public int SpecialState { get; set; }

        public ICollection<Contact> Contact { get; set; }
    }


    [Table("Address")]
    public class Address
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        [Required]
        [DisplayName("Code")]
        [Range(0, 99999)]
        public string Code { get; set; }

        [DisplayName("Client Code")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [DisplayName("AddressType")]
        public int AddressTypeID { get; set; }
        public virtual AddressType AddressType { get; set; }

        public virtual ICollection<AddressLine> Lines { get; set; }

        public bool Active { get; set; }
    }

    [Table("AddressLine")]
    public class AddressLine
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AddressLineId { get; set; }

        [DisplayName("Line Text")]
        public string AddressLineText { get; set; }

        [DisplayName("Address")]
        public int AddressID { get; set; }
        public virtual Address Address { get; set; }

        [DisplayName("Order")]
        public int Order { get; set; }

        public bool Active { get; set; }

    }


    [Table("Contact")]
    public class Contact
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ContactId { get; set; }

        [Required]
        [DisplayName("Client Code")]
        public int ClientID { get; set; }
        [ForeignKey("ClientID")]
        public virtual Client Client { get; set; }

        [DisplayName("Value")]
        public string ContactName { get; set; }

        [Required]
        [DisplayName("Contact Type")]
        public int ContactTypeID { get; set; }
        public virtual ContactType ContactType { get; set; }

        public bool Active { get; set; }

    }


    [Table("Product")]
    public class Product
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        [Required]
        [DisplayName("Product")]
        public string ProductName { get; set; }
        [Required]
        [DisplayName("Product Code")]
        public string ProductCode { get; set; }
        [DisplayName("Evolution Code")]
        public string EvolutionCode { get; set; }

        public bool Active { get; set; }
    }


    [Table("Subscription")]
    public class Subscription
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SubscriptionId { get; set; }

        [Required]
        [DisplayName("Client Type")]
        public int ClientTypeID { get; set; }
        public virtual ClientType ClientType { get; set; }

        [Required]
        [DisplayName("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [DisplayName("Mandatory Quantity")]
        public int MandatoryQuantity { get; set; }

        [Required]
        [DisplayName("Price")]
       // [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [DisplayName("Price Excl")]
        //[DataType(DataType.Currency)]
        public decimal PriceExcl { get; set; }

        [DisplayName("Valid From Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ValidFromDate { get; set; }

        public bool Active { get; set; }

        [DisplayName("Initial Once Off From Account")]
        public Guid InitialOnceOffFromAccountID { get; set; }

        [DisplayName("Sales Type")]
        public int SalesTypeID { get; set; }
    }

    [Table("ClientSubscription")]
    public class ClientSubscription
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ClientSubscriptionId { get; set; }

        [Required]
        [DisplayName("Client Code")]
        public int? ClientID { get; set; }
        public virtual Client Client { get; set; }

        [Required]/*TODO: check logic - I'm using product id instead of subscriptionId. If a client wants products outside of the standards subscription we must be able to accomodate*/
        [DisplayName("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [DisplayName("Quantity")]
        [Range(1, 9999)]
        public int Quantity { get; set; }

        public bool Active { get; set; }
    }

    [Table("BankingDetail")]
    public class BankingDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BankingDetailId { get; set; }

        [Required]
        [DisplayName("Client Code")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [Required]
        [DisplayName("Account Holder")]
        public int AccountHolderID { get; set; }
        public virtual AccountHolder AccountHolder { get; set; }

        [DisplayName("Account Holder Other Detail")]
        public string AccountHolderOtherDetail { get; set; }

        [Required]
        [DisplayName("Initials")]
        public string Initials { get; set; }

        [Required]
        [DisplayName("Surname")]
        public string Surname { get; set; }

        [DisplayName("Email")]
        public string EmailContact { get; set; }

        [DisplayName("Work")]
        public string WorkContact { get; set; }

        [DisplayName("Home")]
        public string HomeContact { get; set; }

        [DisplayName("Cell")]
        public string CellContact { get; set; }

        [DisplayName("Interval Month(s)")]
        public int Interval { get; set; }

        [DisplayName("Next DO Month")]
        public int NextDOMonth { get; set; }

        /*
        doesn't make sense to have this relational on banking detail
        [DisplayName("Email")]
        public int EmailContactID { get; set; }
        public virtual Contact Email { get; set; }

        [DisplayName("Work")]
        public int WorkContactID { get; set; }
        public virtual Contact Work { get; set; }

        [DisplayName("Home")]
        public int HomeContactID { get; set; }
        public virtual Contact Home { get; set; }

        [DisplayName("Work")]
        public int CellContactID { get; set; }
        public virtual Contact Cell { get; set; }
        */
        [Required]
        [DisplayName("Account Type")]
        public int AccountTypeID { get; set; }
        public virtual AccountType AccountType { get; set; }

        [Required]
        [DisplayName("Commencement Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CommencementDate { get; set; }

        [Required]
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }

        [Required]
        [DisplayName("Bank")]
        public int BankID { get; set; }
        public virtual Bank Bank { get; set; }

        [Required]
        [DisplayName("Branch")]
        public int BranchID { get; set; }
        public virtual Branch Branch { get; set; }

        [Required]
        [DisplayName("Salary Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SalaryDate { get; set; }

        public bool Active { get; set; }

    }


    [Table("SystemNote")] /*This table may be used to attatch a note to any database entry with a guid for a key*/
    public class SystemNote
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid NoteId { get; set; }

        [DisplayName("NoteText")]
        public string NoteText { get; set; }

        [DisplayName("User")]
        public Guid UserID { get; set; }

        [DisplayName("Created")]
        public DateTime Created { get; set; }
    }


    [Table("SystemLink")] /*this table may be used to link any two database entries together that has a guid as a key*/
    public class SystemLink
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid LinkId { get; set; }

        public Guid Parent { get; set; }
        public Guid Child { get; set; }

        [DisplayName("User")]
        public Guid UserID { get; set; }

        [DisplayName("Created")]
        public DateTime Created { get; set; }
    }
    #endregion

    #region Financial tables

    [Table("CalculationPeriod")]
    public class CalculationPeriod
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid CalculationPeriodId { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public Guid PreviousPeriodId { get; set; }
        [ForeignKey("PreviousPeriodId")]
        public virtual CalculationPeriod PreviousPeriod { get; set; }
        public int PeriodMonth { get; set; }
        public int PeriodYear { get; set; }
        public bool PeriodOpen { get; set; }
        public bool ForceClose { get; set; }
        public bool LevelProcessed { get;  set; }
        public bool GroupProcessed { get;  set; }
    }

    [Table("GroupSaleRebateCalculationSummary")]
    public class GroupSaleRebateCalculationSummary
    {
        [Key]
        public Guid GroupSaleRebateCalculationSummaryId { get; set; }

        public Guid CalculationPeriodId { get; set; }
        [ForeignKey("CalculationPeriodId")]
        public virtual CalculationPeriod CalculationPeriod { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public int ClientTypeId { get; set; }
        [ForeignKey("ClientTypeId")]
        public ClientType ClientType { get; set; }

        [DataType(DataType.Currency)]
        public decimal LevelAmount { get; set; }

        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        
        public decimal LevelAmountPer { get; set; }
        [DataType(DataType.Currency)]
        public decimal CalculatedAmount { get; set; }

        public int Lvl { get; set; }
        public int ReletiveLvl { get; set; }
        
    }

    [Table("GroupRebateCalculationRevenueSummary")]
    public class GroupRebateCalculationRevenueSummary
    {

        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid GroupSaleRebateCalculationRevenueSummaryId { get; set; }

        public Guid GroupSaleRebateCalculationSummaryId { get; set; }
        [ForeignKey("GroupSaleRebateCalculationSummaryId")]
        public GroupSaleRebateCalculationSummary GroupSaleRebateCalculationSummary { get; set; }

        
        public Guid GroupSaleRebateCalculationRevenueId { get; set; }
        [ForeignKey("GroupSaleRebateCalculationRevenueId")]
        public GroupSaleRebateCalculationRevenue GroupSaleRebateCalculationRevenue { get; set; }
    }

    [Table("GroupSaleRebateCalculationRevenue")]
    public class GroupSaleRebateCalculationRevenue
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid GroupSaleRebateCalculationRevenueId { get; set; }

        public Guid OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; }

        [DataType(DataType.Currency)]
        public decimal SaleAmount { get; set; }
        
        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public int ClientTypeId { get; set; }
        [ForeignKey("ClientTypeId")]
        public ClientType ClientType { get; set; }

    }


    /// <summary>
    /// old calculation method
    /// </summary>
    [Table("GroupSaleRebateCalculationOrderHeader")]
    public class GroupSaleRebateCalculationOrderHeader
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid GroupSaleRebateCalculationOrderHeaderId { get; set; }

        public Guid GroupSaleRebateCalculationId { get; set; }
        [ForeignKey("GroupSaleRebateCalculationId")]
        public GroupSaleRebateCalculation GroupSaleRebateCalculation { get; set; }

        public Guid OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId ")]
        public OrderHeader OrderHeader { get; set; }

    }

        [Table("GroupSaleRebateCalculation")]
    public class GroupSaleRebateCalculation
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid GroupSaleRebateCalculationId { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public int Level { get; set; }

        public int ClientTypeId { get; set; }
        [ForeignKey("ClientTypeId")]
        public ClientType ClientType { get; set; }

        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public Guid CalculationPeriodId { get; set; }
        [ForeignKey("CalculationPeriodId")]
        public virtual CalculationPeriod CalculationPeriod { get; set; }
        public int? ResellerID { get; set; }
    }
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    [Table("SubscriptionSaleRebateCalculation")]
        public class SubscriptionSaleRebateCalculation
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public Guid SubscriptionSaleRebateCalculationId { get; set; }

            public Guid OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; }

        public decimal FirstProductPer { get; set; }
        public decimal OtherProductsPer { get; set; }

        [DataType(DataType.Currency)]
        public decimal FirstProductAmount { get; set; }
        [DataType(DataType.Currency)]
        public decimal OtherProductsAmount { get; set; }

        [DataType(DataType.Currency)]
        public decimal FirstProductCalculatedAmount { get; set; }
        [DataType(DataType.Currency)]
        public decimal OtherProductsCalculatedAmount { get; set; }

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public Client Client { get; set; }
                
        public int Level { get; set; }

        public int ReletiveClientId { get; set; }
        [ForeignKey("ReletiveClientId")]
        public Client ReletiveClient { get; set; }

        public int ClientTypeId { get; set; }
        [ForeignKey("ClientTypeId")]
        public ClientType ClientType { get; set; }

        public Guid CalculationPeriodId { get; set; }
        [ForeignKey("CalculationPeriodId")]
        public virtual CalculationPeriod CalculationPeriod { get; set; }
        public int RebateLevelsTableRowId { get; set; }
        public Guid FromAccount { get;  set; }
        public Guid ToAccount { get;  set; }
    }

      [Table("Rebate")]
    public class Rebate
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int RebateId { get; set; }
        [DisplayName("Client Type")]
        public int ClientTypeId { get; set; }
        public virtual ClientType ClientType { get; set; }
        [DisplayName("Product")]
        public int ProductID { get; set; }
        public virtual Product Product {get;set;}
        [DisplayName("Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        [DisplayName("Account")]
        public Guid AccountId { get; set; }
        public virtual finAccount Account { get; set; }
    }

    [Table("finDebitCreditRule")]
    public class DebitCreditRule
    {
        [Key]
        public Guid AccountId { get; set; }
        [DataType(DataType.Currency)]
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        [ForeignKey("AccountId")]
        public virtual finAccount Account { get; set; }
        public bool ActiveDebit { get; set; }
        public bool ActiveCredit { get; set; }
    }

    [Table("finAccount")]
    public class finAccount
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid AccountId { get; set; }
        [DisplayName("Account")]
        public string AccountName { get; set; }
        [DisplayName("System Account")]
        public bool IsSystemAccount { get; set; }
        public bool Active { get; set; }
        
    }


        [Table("DebitOrderPeriodDetail")]
    public class DebitOrderPeriodDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DebitOrderPeriodDetailID { get; set; }
        [DisplayName("Client Code")]
        public int ClientID { get; set; }
        public int Period { get; set; }
        [DisplayName("Product")]
        public int ProductID { get; set; }
        [DisplayName("Price")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        [DisplayName("Status")]
        public int DebitOrderPeriodStatusID { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }
    }

    [Table("PublicHolidays")]
    public class PublicHolidays
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [DisplayName("Holiday Date")]
        public DateTime HolidayDate { get; set; }
        [DisplayName("Description")]
        public string HolidayDescription { get; set; }
    }

    [Table("DebitOrderPeriodStatus")]
    public class DebitOrderPeriodStatus
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DebitOrderPeriodStatusID { get; set; }
        public string Description { get; set; }
    }

        [Table("finClientAccount")]
    public class finClientAccount
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid ClientAccountId { get; set; }

        [DisplayName("Client Code")]
        public int ClientID { get; set; } 
        public virtual Client Client { get; set; }

        [DisplayName("Account")]
        public Guid AccountId { get; set; }
        public virtual finAccount Account { get; set; }

        public bool Active { get; set; }

    }

    [Table("finJournal")]
    public class finJournal
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid JournalId { get; set; }

        [DisplayName("Account")]
        public Guid AccountID { get; set; } /*account will normaly point to a client account id but may point to account id when it is a system account*/

        [DisplayName("Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [DisplayName("Corresponding Joutnal Entry")]
        public Guid CorrespondingJournalId { get; set; }

        [DisplayName("Movement Source")]
        public Guid MovementSource { get; set; }

        [DisplayName("User")]
        public Guid UserID { get; set; }

        [DisplayName("Effective Date")]
        public DateTime EffectiveDate { get; set; }

        [DisplayName("Journal Date")]
        public DateTime JournalDate { get; set; }

        [DisplayName("Comment")]
        public string Comment { get; set; }

        public int Index { get; set; }

        public bool Active { get; set; }

    }
    #endregion

    #region Debit order

    [Table("DebitOrder")]
    public class DebitOrder
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid DebitOrderId { get; set; }

        [DisplayName("Client Code")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [Required]
        [DisplayName("Account Holder")]
        public int AccountHolderID { get; set; }
        public virtual AccountHolder AccountHolder { get; set; }

        [DisplayName("Account Holder Other Detail")]
        public string AccountHolderOtherDetail { get; set; }

        [Required]
        [DisplayName("Initials")]
        public string Initials { get; set; }

        [Required]
        [DisplayName("Surname")]
        public string Surname { get; set; }

        [Required]
        [DisplayName("Account Type")]
        public int AccountTypeID { get; set; }
        public virtual AccountType AccountType { get; set; }

        [Required]
        [DisplayName("Debit Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DebitDate { get; set; }

        [Required]
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }

        [Required]
        [DisplayName("Bank")]
        public int BankID { get; set; }
        public virtual Bank Bank { get; set; }

        [Required]
        [DisplayName("Branch")]
        public int BranchID { get; set; }
        public virtual Branch Branch { get; set; }

        [Required]
        [DisplayName("Source")]
        public Guid SourceID { get; set; }

        public bool Active { get; set; }
        public DateTime Created { get; set; }
        public DateTime? ProcessDate { get; set; }
        public decimal Amount { get; set; }
        //public int ExternalDebitOrderId { get; set; }

    }


    #endregion

    #region Loging and settings
    [Table("SystemSetting")]
    public class SystemSetting
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SettingId { get; set; }

        public string SettingKey { get; set; }
        public string SettingValue { get; set; }

    }

    [Table("lgActivity")]
    public class LGActivity
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ActivityLogId { get; set; }

        public string Activity { get; set; }

    }

    [Table("lgActivityLog")]
    public class LGActivityLog
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Guid ActivityLogId { get; set; }

        public int ActivityId { get; set; }
        
        public int? IntId { get; set; }
        
        public Guid? GuidId { get; set; }
        
        public string Note { get; set; } 

        public DateTime iDate { get; set; }
    }

        #endregion
    }

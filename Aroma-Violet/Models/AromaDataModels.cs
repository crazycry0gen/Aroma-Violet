using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Globalization;
using System.Web.Security;

namespace Aroma_Violet.Models
{
    public class AromaContext : DbContext
    {
        public AromaContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<ClientType> ClientTypes { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Titel> Titels { get; set; }
        public DbSet<EthnicGroup> EthnicGroups { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<IncomeGroup> IncomeGroups { get; set; }
        public DbSet<AddressType> AddressTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressLine> AddressLines { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ContactType> ContactTypes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<AccountHolder> AccountHolders { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public System.Data.Entity.DbSet<Aroma_Violet.Models.Product> Products { get; set; }
        public System.Data.Entity.DbSet<Aroma_Violet.Models.BankingDetail> BankingDetails { get; set; }
        public System.Data.Entity.DbSet<Aroma_Violet.Models.Subscription> Subscriptions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
        }
    }

    #region Lookup tables

    [Table("ClientType")]
    public class ClientType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ClientTypeId { get; set; }
        [Required]
        [DisplayName("Client Type")]
        public string ClientTypeName { get; set; }
        public bool Active { get; set; }

        /*public virtual ICollection<Client> Clients { get; set; }*/
    }

    [Table("Titel")]
    public class Titel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TitelId { get; set; }
        [Required]
        [DisplayName("Titel")]
        public string TitelName { get; set; }
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

    }
    #endregion


    [Table("Client")]
    public class Client
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }

        [Required]
        [DisplayName("Name")]
        public string ClientName { get; set; }

        [DisplayName("Nick Name")]
        public string NickName { get; set; }

        [DisplayName("Full Names")]
        public string FullNames { get; set; }

        [Required]
        [DisplayName("Home Language")]
        public int HomeLanguageID { get; set; }
        public virtual Language HomeLanguage { get; set; }

        [Required]
        [DisplayName("Employer")]
        public string Employer { get; set; }

        [Required]
        [DisplayName("Date of Birth")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
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
        [DisplayName("Titel")]
        public int TitelID { get; set; }
        public virtual Titel Titel { get; set; }

        [DisplayName("Ethnic Group")]
        public int EthnicGroupID { get; set; }
        public virtual EthnicGroup EthnicGroup { get; set; }

        [DisplayName("Income Group")]
        public int IncomeGroupID { get; set; }
        public virtual IncomeGroup IncomeGroup { get; set; }

        [Required]
        [DisplayName("Postal Address")]
        public int? PostalAddressID { get; set; }
        public virtual Address PostalAddress { get; set; }

        [DisplayName("Delivery Address")]
        public int? DeliveryAddressID { get; set; }
        public virtual Address DeliveryAddress { get; set; }

        [Required]
        [DisplayName("Province")]
        public int? ProvinceID { get; set; }
        public virtual Province Province { get; set; }

        [Required]
        [DisplayName("Country")]
        public int CountryID { get; set; }
        public virtual Country Country { get; set; }
    }


    [Table("Address")]
    public class Address
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AddressId { get; set; }

        [Required]
        [DisplayName("Code")]
        [Range(100, 99999)]
        public string Code { get; set; }

        [DisplayName("Client")]
        public string ClientID { get; set; }
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
        [Required]
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
        [DisplayName("Client")]
        public int ClientID { get; set; }
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
        public int ProductId { get; set; }
        [Required]
        [DisplayName("Product")]
        public string ProductName { get; set; }
        [Required]
        [DisplayName("UnitPrice")]
        public double UnitPrice { get; set; }
        public bool Active { get; set; }
    }


    [Table("ClientSubscription")]
    public class Subscription
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int SubscriptionId { get; set; }

        [Required]
        [DisplayName("Client")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [Required]
        [DisplayName("Product")]
        public int ProductID { get; set; }
        public virtual Product Product { get; set; }

        [Required]
        [DisplayName("Quantity")]
        public int Quantity { get; set; }

        public bool Active { get; set; }
    }


    [Table("BankingDetail")]
    public class BankingDetail
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int BankingDetailId { get; set; }


        [DisplayName("Client")]
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

        [Required]
        [DisplayName("Account Type")]
        public int AccountTypeID { get; set; }
        public virtual AccountType AccountType { get; set; }

        [Required]
        [DisplayName("Commencement Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
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
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SalaryDate { get; set; }

        public bool Active { get; set; }

    }
}

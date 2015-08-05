using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
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
    }

    #region Lookup tables

    [Table("ClientType")]
    public class ClientType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ClientTypeId { get; set; }
        [DisplayName("Name")]
        public string ClientTypeName { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<Client> Clients { get; set; }
    }

    [Table("Titel")]
    public class Titel
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int TitelId { get; set; }
        [DisplayName("Name")]
        public string TitelName { get; set; }
        public bool Active { get; set; }

    }


    [Table("EthnicGroup")]
    public class EthnicGroup
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int EthnicGroupId { get; set; }
        [DisplayName("Name")]
        public string EthnicGroupName { get; set; }
        public bool Active { get; set; }

    }


    [Table("Language")]
    public class Language
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LanguageId { get; set; }
        [DisplayName("Name")]
        public string LanguageName { get; set; }
        public bool Active { get; set; }

    }


    [Table("IncomeGroup")]
    public class IncomeGroup
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int IncomeGroupId { get; set; }
        [DisplayName("Name")]
        public string IncomeGroupName { get; set; }
        public bool Active { get; set; }

    }

    [Table("AddressType")]
    public class AddressType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int AddressTypeId { get; set; }
        [DisplayName("Name")]
        public string AddressTypeName { get; set; }
        public bool Active { get; set; }

    }

    [Table("Province")]
    public class Province
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ProvinceId { get; set; }
        [DisplayName("Name")]
        public string ProvinceName { get; set; }
        public bool Active { get; set; }

    }

    [Table("Country")]
    public class Country
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CountryId { get; set; }
        [DisplayName("Name")]
        public string CountryName { get; set; }
        public bool Active { get; set; }

    }

    [Table("ContactType")]
    public class ContactType
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ContactTypeId { get; set; }
        [DisplayName("Name")]
        public string ContactTypeName { get; set; }
        public bool Active { get; set; }

    }
    #endregion


    [Table("Client")]
    public class Client
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }

        [DisplayName("Name")]
        public string ClientName { get; set; }

        [DisplayName("Nick Name")]
        public string NickName { get; set; }

        [DisplayName("Full Names")]
        public string FullNames { get; set; }

        [DisplayName("Home Language")]
        public int HomeLanguageID { get; set; }
        public Language HomeLanguage { get; set; }

        [DisplayName("Employer")]
        public string Employer { get; set; }

        [DisplayName("Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Surname")]
        public string ClientSurname { get; set; }

        [DisplayName("SA Resident")]
        public bool SAResident { get; set; }

        [DisplayName("ID Number")]
        public string IDNumber { get; set; }

        [DisplayName("Client Type")]
        public int ClientTypeID { get; set; }
        public virtual ClientType ClientType { get; set; }

        [DisplayName("Titel")]
        public int TitelID { get; set; }
        public virtual Titel Titel { get; set; }

        [DisplayName("Ethnic Group")]
        public int EthnicGroupID { get; set; }
        public virtual EthnicGroup EthnicGroup { get; set; }

        [DisplayName("Income Group")]
        public int IncomeGroupID { get; set; }
        public virtual IncomeGroup IncomeGroup { get; set; }
        
        [DisplayName("Postal Address")]
        public int? PostalAddressID { get; set; }
        public virtual Address PostalAddress { get; set; }

        [DisplayName("Delivery Address")]
        public int? DeliveryAddressID { get; set; }
        public virtual Address DeliveryAddress { get; set; }
        
        [DisplayName("Province")]
        public int? ProvinceID { get; set; }
        public virtual Province Province { get; set; }

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

        [DisplayName("Code")]
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
        
        [DisplayName("Client")]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }

        [DisplayName("Value")]
        public string ContactName { get; set; }
        
        [DisplayName("Contact Type")]
        public int ContactTypeID { get; set; }
        public virtual ContactType ContactType { get; set; }

        public bool Active { get; set; }

    }
}

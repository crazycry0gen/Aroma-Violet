using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace Aroma_Indigo.Models
{
    public class AromaContext : DbContext
    {
        public AromaContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<ClientType> ClientTypes { get; set; }
        public DbSet<Client> Clients { get; set; }
    }

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

    [Table("Client")]
    public class Client
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ClientId { get; set; }
        [DisplayName("Client Name")]
        public string ClientName { get; set; }
        [DisplayName("Client Surname")]
        public string ClientSurname { get; set; }
        [DisplayName("ID Number")]
        public string IDNumber { get; set; }

        public int ClientTypeID { get; set; }
        [DisplayName("Client Type")]
        public virtual ClientType ClientType { get; set; }
    }
}

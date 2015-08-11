namespace Aroma_Violet.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Aroma_Violet.Models.AromaContext>
    {
        public Configuration()
        {
            AutomaticMigrationDataLossAllowed = true;
            AutomaticMigrationsEnabled = true;
            ContextKey = "Aroma_Violet.Models.AromaContext";
        }

        protected override void Seed(Aroma_Violet.Models.AromaContext context)
        {
            
            context.Titels.AddOrUpdate(
                p => p.TitelName,
                new Aroma_Violet.Models.Titel() {TitelName="Mr",Active=true },
                new Aroma_Violet.Models.Titel() { TitelName = "Mrs", Active = true }
                );

            context.EthnicGroups.AddOrUpdate(
                p => p.EthnicGroupName,
                new Aroma_Violet.Models.EthnicGroup() {EthnicGroupName="Black", Active=true },
                new Aroma_Violet.Models.EthnicGroup() {EthnicGroupName="Coloured", Active=true },
                new Aroma_Violet.Models.EthnicGroup() {EthnicGroupName="Indian", Active=true },
                new Aroma_Violet.Models.EthnicGroup() {EthnicGroupName="White", Active=true }
                );

            context.Languages.AddOrUpdate(
                p=>p.LanguageName,
                new Aroma_Violet.Models.Language() { LanguageName = "Zulu", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Xhosa", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Afrikaans", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "English", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Northern Sotho", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Tswana", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Sotho", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Tsonga", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Swazi", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Venda", Active = true },
                new Aroma_Violet.Models.Language() { LanguageName = "Ndebele", Active = true }
                );

            context.IncomeGroups.AddOrUpdate(
               p => p.IncomeGroupName,
               new Aroma_Violet.Models.IncomeGroup() { IncomeGroupName = "R1-R6000", Active = true },
               new Aroma_Violet.Models.IncomeGroup() { IncomeGroupName = "R6001-R10000", Active = true },
               new Aroma_Violet.Models.IncomeGroup() { IncomeGroupName = "R10001-R15000", Active = true },
               new Aroma_Violet.Models.IncomeGroup() { IncomeGroupName = "R15000-or higher", Active = true }
               );

            context.AddressTypes.AddOrUpdate(
               p => p.AddressTypeName,
               new Aroma_Violet.Models.AddressType() { AddressTypeName = "Postal", Active = true },
               new Aroma_Violet.Models.AddressType() { AddressTypeName = "Physical", Active = true }
               );

            context.Provinces.AddOrUpdate(
                p=>p.ProvinceName,
                new Aroma_Violet.Models.Province() { ProvinceName = "The Eastern Cape", Active = true },
                new Aroma_Violet.Models.Province() { ProvinceName = "The Free State", Active = true },
                new Aroma_Violet.Models.Province() { ProvinceName = "Gauteng", Active = true },
                new Aroma_Violet.Models.Province() { ProvinceName = "KwaZulu-Natal", Active = true },
                new Aroma_Violet.Models.Province() { ProvinceName = "Limpopo", Active = true },
                new Aroma_Violet.Models.Province() { ProvinceName = "Mpumalanga", Active = true },
                new Aroma_Violet.Models.Province() { ProvinceName = "The Northern Cape", Active = true },
                new Aroma_Violet.Models.Province() { ProvinceName = "North West", Active = true },
                new Aroma_Violet.Models.Province() { ProvinceName = "The Western Cape", Active = true }
                );

            context.Countries.AddOrUpdate(
                p=>p.CountryName,
                new Aroma_Violet.Models.Country() {CountryName="South Africa",Active=true }
                );

            context.ContactTypes.AddOrUpdate(
                p=>p.ContactTypeName,
                new Aroma_Violet.Models.ContactType() {ContactTypeName="Tel (Home)",Active=true },
                new Aroma_Violet.Models.ContactType() { ContactTypeName = "Tel (Work)", Active = true },
                new Aroma_Violet.Models.ContactType() { ContactTypeName = "Cell", Active = true },
                new Aroma_Violet.Models.ContactType() { ContactTypeName = "Fax (Home)", Active = true },
                new Aroma_Violet.Models.ContactType() { ContactTypeName = "Fax (Work)", Active = true },
                new Aroma_Violet.Models.ContactType() { ContactTypeName = "EMail", Active = true }
                );
            
            context.AccountHolders.AddOrUpdate(
                p=>p.AccountHolderName,
                new Aroma_Violet.Models.AccountHolder() {AccountHolderName="Self", Active=true },
                new Aroma_Violet.Models.AccountHolder() { AccountHolderName = "Spouse", Active = true },
                new Aroma_Violet.Models.AccountHolder() { AccountHolderName = "Child", Active = true },
                new Aroma_Violet.Models.AccountHolder() { AccountHolderName = "Parent", Active = true },
                new Aroma_Violet.Models.AccountHolder() { AccountHolderName = "Other", Active = true }
                );

            context.AccountTypes.AddOrUpdate(
                p=>p.AccountTypeName,
                new Aroma_Violet.Models.AccountType() {AccountTypeName= "Cheque", Active = true},
                new Aroma_Violet.Models.AccountType() { AccountTypeName = "Debitable Savings", Active = true },
                new Aroma_Violet.Models.AccountType() { AccountTypeName = "Transaction", Active = true }
                );

            context.Banks.AddOrUpdate(
                p=>p.BankName,
                new Aroma_Violet.Models.Bank() {BankName= "ABSA", Active = true },
                new Aroma_Violet.Models.Bank() { BankName = "FNB", Active = true },
                new Aroma_Violet.Models.Bank() { BankName = "Netbank", Active = true },
                new Aroma_Violet.Models.Bank() { BankName = "Standard bank", Active = true }
                );
        }
    }
}

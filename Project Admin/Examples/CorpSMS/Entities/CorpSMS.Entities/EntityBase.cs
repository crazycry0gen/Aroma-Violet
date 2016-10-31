using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Validation;

namespace StratCorp.CorpSMS.Entities
{
    public class EntityBase
    {
        [XmlIgnore]
        public bool IsValid { get; set; }

        public string ValidationReason { get; set; }

        public void Validate<T>()
        {
            var validator = GetValidator<T>();

            ValidationResults results = validator.Validate(this);

            this.IsValid = results.IsValid;
            if (!results.IsValid)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Validation on entity failed:");
                results.ForAll(r =>
                {
                    builder.AppendLine(string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}: {1}",
                        r.Key,
                        r.Message));
                });
                this.ValidationReason = builder.ToString();
            }
        }

        private static Validator<T> GetValidator<T>()
        {
            ValidatorFactory valFactory = EnterpriseLibraryContainer.Current.GetInstance<ValidatorFactory>();
            return valFactory.CreateValidator<T>();
        }
    }
}

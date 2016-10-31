using System.Configuration;

namespace StratCorp.CorpSMS.Infrastructure.Helpers
{
    public static class ConfigurationHelper
    {
        public static T GetConfigurationSection<T>(string sectionName) where T : ConfigurationSection
        {
            //TODO: Add try catch
            ConfigurationSection section = ConfigurationManager.GetSection(sectionName) as ConfigurationSection;
            if (section != null)
            {
                return section as T;
            }
            else
            {
                return null;
            }
        }       
    }
}

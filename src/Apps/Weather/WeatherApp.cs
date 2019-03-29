using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.IOInterfaces.Interfaces.Weather;
using SAMI.Persistence;

namespace SAMI.Apps.Weather
{
    [ParseableElement("Weather", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class WeatherApp : VoiceActivatedApp<WeatherConversation>
    {
        public override bool IsValid
        {
            get
            {
                return ConfigManager != null && ConfigManager.FindAllComponentsOfType<IWeatherSensor>().Any();
            }
        }

        public override string InvalidMessage
        {
            get
            {
                return "There are no sources of weather data currently installed. Please add an IO Resource which can supply weather data.";
            }
        }

        public WeatherApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (IsValid)
            {
                Provider = new GrammarProvider(new XmlGrammar(ConfigManager, "WeatherGrammar.grxml", GetType()));
            }
        }
    }
}

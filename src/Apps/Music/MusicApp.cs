using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Music
{
    [ParseableElement("Music", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class MusicApp : VoiceActivatedApp<MusicConversation>
    {
        private bool _playlistsFound = true;
        public override bool IsValid
        {
            get
            {
                return ConfigManager != null && ConfigManager.FindAllComponentsOfType<IMusicController>().Any() && _playlistsFound;
            }
        }

        public override String InvalidMessage
        {
            get
            {
                if (IsValid)
                {
                    return String.Empty;
                }
                else
                {
                    return "Spotify has encountered some issues with connecting to the service. Please try again later.";
                }
            }
        }

        public MusicApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            List<String> playlists = ConfigManager.FindAllComponentsOfType<IMusicController>().SelectMany(s => s.GetPlaylists()).ToList();
            Provider = new GrammarProvider(GetMainGrammar, new TimeSpan(1, 0, 0, 0));
        }

        internal XmlGrammar GetMainGrammar()
        {
            if (ConfigManager.FindAllComponentsOfType<IMusicController>().Any())
            {
                List<String> playlists = ConfigManager.FindAllComponentsOfType<IMusicController>().SelectMany(s => s.GetPlaylists()).ToList();

                if (playlists.Any())
                {
                    return GrammarUtility.CreateGrammarFromList(ConfigManager.GetPathForFile("MusicGrammar.xml", GetType()), "PlaylistName", playlists);
                }
                else
                {
                    _playlistsFound = false;
                }
            }
            return null;
        }
    }
}

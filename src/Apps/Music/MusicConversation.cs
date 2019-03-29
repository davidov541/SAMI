using System;
using System.Collections.Generic;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Music
{
    internal class MusicConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "music";
            }
        }

        public MusicConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override string Speak()
        {
            base.Speak();
            Dialog phrase = CurrentDialog;
            ConversationIsOver = true;
            IEnumerable<IMusicController> musicSources = ConfigManager.FindAllComponentsOfType<IMusicController>();
            IMusicController controller;
            switch (phrase.GetPropertyValue("Subcommand"))
            {
                case "playrandom":
                    return StartPlaylist(phrase.GetPropertyValue("Playlist"), true);
                case "play":
                    return StartPlaylist(phrase.GetPropertyValue("Playlist"), false);
                case "stop":
                    controller = musicSources.FirstOrDefault(s => s.State == PlayerState.Playing);
                    if(controller != null)
                    {
                        controller.StopMusic();
                    }
                    break;
                case "playlists":
                    List<String> playlists = musicSources.SelectMany(s => s.GetPlaylists()).ToList();
                    if(playlists.Any())
                    {
                        return String.Format("Currently, the available playlists are {0}.", SayList(playlists.ToList()));
                    }
                    else
                    {
                        return "No playlists were found.";
                    }
                case "deletesong":
                    controller = musicSources.FirstOrDefault(s => s.State == PlayerState.Playing);
                    if (controller != null)
                    {
                        controller.DeleteCurrentTrack();
                        controller.SkipToNextTrack();
                    }
                    else
                    {
                        return String.Empty;
                    }
                    break;
                case "skip":
                    controller = musicSources.FirstOrDefault(s => s.State == PlayerState.Playing);
                    if(controller != null)
                    {
                        controller.SkipToNextTrack();
                    }
                    else
                    {
                        return String.Empty;
                    }
                    break;
                case "song":
                    foreach (IMusicController c in musicSources.Where(s => s.State == PlayerState.Playing))
                    {
                        String currentSong = c.GetCurrentSongName();
                        String currentArtist = c.GetCurrentArtistName();
                        if (currentSong != null && currentArtist != null)
                        {
                            return String.Format("This is {0} by {1}.", currentSong, currentArtist);
                        }
                    }
                    return String.Empty;
                    break;
                case "artist":
                    foreach (IMusicController c in musicSources.Where(s => s.State == PlayerState.Playing))
                    {
                        String currentArtist = c.GetCurrentArtistName();
                        if (currentArtist != null)
                        {
                            return String.Format("This is by {0}.", currentArtist);
                        }
                    }
                    return String.Empty;
                    break;
                default:
                    break;
            }
            return "O K";
        }

        private String StartPlaylist(String playlist, bool random)
        {
            IMusicController controller = ConfigManager.FindAllComponentsOfType<IMusicController>().FirstOrDefault(c => c.GetPlaylists().Contains(playlist));
            if (controller == null || !controller.TryStartPlaylist(playlist, random))
            {
                return "Could not find that playlist. Please ask for a different playlist, or ask me for one of the available playlists.";
            }
            else
            {
                return "O K";
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Podcast
{
    internal class PodcastConversation : Conversation
    {
        private PodcastInfo _info;
        private IEnumerable<PodcastInfo> _podcasts;
        private IMediaFilePlayer _player;

        protected override String CommandName
        {
            get
            {
                return "podcast";
            }
        }

        public PodcastConversation(IConfigurationManager configManager, IEnumerable<PodcastInfo> podcasts)
            : base(configManager)
        {
            _podcasts = podcasts;
            _player = ConfigManager.FindAllComponentsOfType<IMediaFilePlayer>().FirstOrDefault();
        }

        public override string Speak()
        {
            base.Speak();
            ConversationIsOver = true;
            Dialog phrase = CurrentDialog;
            if (_player == null)
            {
                return String.Empty;
            }
            PlayerState currentState = _player.CurrentState;
            switch (phrase.GetPropertyValue("Subcommand"))
            {
                case "playnext":
                    if (currentState != PlayerState.Stopped)
                    {
                        _player.StopAudioFile();
                    }
                    _info = _podcasts.Single(i => i.Name.Equals(phrase.GetPropertyValue("Podcast")));
                    if (currentState == PlayerState.Stopped)
                    {
                        _player.TrackFinished += Player_EndOfStream;
                    }
                    _player.PlayAudioFile(_info.NextAudioFileLocation.AbsoluteUri);
                    break;
                case "pause":
                    if (currentState == PlayerState.Playing)
                    {
                        _player.PauseAudioFile();
                    }
                    break;
                case "resume":
                    if (currentState == PlayerState.Paused)
                    {
                        _player.ResumeAudioFile();
                    }
                    break;
            }
            return String.Empty;
        }

        private void Player_EndOfStream(Object sender, EventArgs e)
        {
            _info.NextEpisodeFinished();
            _player.TrackFinished -= Player_EndOfStream;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Database;
using SAMI.Persistence;

namespace SAMI.Apps.Podcast
{
    [ParseableElement("Podcast", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class PodcastInfo : IParseable
    {
        private Uri _rssFeedLocation;
        private bool _isOldestFirst;
        private List<Episode> _episodes;
        private IConfigurationManager _configManager;

        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                return NextAudioFileLocation != null;
            }
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("Name", () => Name, name => Name = name);
                yield return new PersistentProperty("Url", () => _rssFeedLocation.AbsolutePath, location => SetURLFromString(location));
                yield return new PersistentProperty("IsOldestFirst", () => _isOldestFirst.ToString(), isOldestFirst => _isOldestFirst = Boolean.Parse(isOldestFirst));
            }
        }

        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public String Name
        {
            get;
            private set;
        }

        private DateTime NextDateTime
        {
            get
            {
                DateTime targetTimestamp = DateTime.Now;
                if (_isOldestFirst)
                {
                    targetTimestamp = _episodes.Where(ep => !ep.PreviouslySeen).Min(e => e.Timestamp);
                }
                else
                {
                    targetTimestamp = _episodes.Where(ep => !ep.PreviouslySeen).Max(e => e.Timestamp);
                }
                return targetTimestamp;
            }
        }

        public Uri NextAudioFileLocation
        {
            get
            {
                if (_episodes == null)
                {
                    return null;
                }
                return _episodes.Single(ep => ep.Timestamp.Equals(NextDateTime)).AudioFile;
            }
        }

        public PodcastInfo()
        {
        }

        public PodcastInfo(String name, Uri feedLocation, bool isOldestFirst)
            : this()
        {
            Name = name;
            _rssFeedLocation = feedLocation;
            _isOldestFirst = isOldestFirst;
        }

        public void Initialize(IConfigurationManager manager)
        {
            _configManager = manager;
            UpdateEpisodes();
        }

        public void Dispose()
        {
        }

        private void SetURLFromString(String url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out _rssFeedLocation))
            {
                _rssFeedLocation = new Uri(Directory.GetCurrentDirectory() + "/" + url);
            }
        }

        private void UpdateEpisodes()
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(_rssFeedLocation.AbsoluteUri);
            }
            catch (WebException)
            {
                // The URL is not reachable. In this case, just return.
                _episodes = null;
                return;
            }
            catch (FileNotFoundException)
            {
                _episodes = null;
                return;
            }
            _episodes = new List<Episode>();
            foreach (XmlNode node in doc.SelectNodes("rss/channel/item"))
            {
                DateTime pubDate = DateTime.Parse(node.SelectSingleNode("pubDate").InnerText);
                _episodes.Add(new Episode(new Uri(node.SelectSingleNode("link").InnerText), pubDate));
            }

            IDatabaseManager man = _configManager.FindAllComponentsOfType<IDatabaseManager>().FirstOrDefault(m => m.Name.Equals("SQL"));
            if (man != null)
            {
                man.StartSession();
                IDataReader reader;
                if (man.TryRunResultQuery("SELECT * FROM dbo.Podcasts", out reader))
                {
                    while (!reader.IsClosed && reader.Read())
                    {
                        if (((String)reader[1]).Equals(_rssFeedLocation.AbsoluteUri))
                        {
                            Episode ep = _episodes.SingleOrDefault(e => e.Timestamp.Equals((DateTime)reader[2]));
                            if (ep != null)
                            {
                                ep.PreviouslySeen = true;
                            }
                        }
                    }
                    reader.Close();
                }
                man.EndSession();
            }
        }

        public void NextEpisodeFinished()
        {
            IDatabaseManager man = _configManager.FindAllComponentsOfType<IDatabaseManager>().FirstOrDefault(c => c.Name.Equals("SQL"));
            if (man != null)
            {
                man.StartSession();
                man.TryRunNoResultQuery(String.Format("INSERT INTO Podcasts VALUES ('{0}','{1}');", _rssFeedLocation.AbsoluteUri, NextDateTime.ToString()));
                man.EndSession();
            }
            UpdateEpisodes();
        }

        public void AddChild(IParseable component)
        {
        }
    }
}

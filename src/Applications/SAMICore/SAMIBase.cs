using System;
using System.Collections.Generic;
using SAMI.Apps;
using SAMI.Error;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Logging;

namespace SAMI.Application
{
    internal abstract class SAMIBase
    {
        private AppCollection _collection;
        private Queue<Conversation> _conversationQueue;

        protected IInternalConfigurationManager ConfigManager
        {
            get;
            private set;
        }

        protected DateTime _lastTimeInteracted;
        protected bool _shouldStop;

        public bool CanBeUpdated
        {
            get;
            protected set;
        }

        public SAMIBase(IInternalConfigurationManager configManager)
        {
            ConfigManager = configManager;

            _collection = new AppCollection(ConfigManager);
            _collection.Init(HandleAsyncAlert);

            _conversationQueue = new Queue<Conversation>();

            CanBeUpdated = false;
        }

        public virtual void Dispose()
        {
            ConfigManager.SaveConfiguration();
        }

        internal void HandleAsyncAlert(object sender, AsyncAlertEventArgs args)
        {
            _conversationQueue.Enqueue(args.StartedConversation);
        }

        public abstract void CheckForInputs();

        public virtual void CheckForUpdates()
        {
            foreach (IVoiceSensor sensor in ConfigManager.FindAllComponentsOfType<IVoiceSensor>())
            {
                sensor.CheckForUpdates();
            }
        }

        public void Run(bool sayIntro)
        {
            InitializeKinect();

            if (sayIntro)
            {
                Speak("I am Sammie, and I am at your disposal. How may I help you?");
            }
            while (!_shouldStop)
            {
                RunSingleCycle();
            }
        }

        public void InitializeKinect()
        {
            foreach (IVoiceSensor sensor in ConfigManager.FindAllComponentsOfType<IVoiceSensor>())
            {
                sensor.RecognizedNewPhrase += NewInputFound;
            }
        }

        public void RunSingleCycle()
        {
            try
            {
                if (_conversationQueue.Count > 0)
                {
                    Conversation conv = _conversationQueue.Peek();
                    if (conv.ReadyToSpeak)
                    {
                        Speak(conv.Speak());
                    }
                    else
                    {
                        CheckForInputs();
                    }
                    if (conv.ConversationIsOver || conv.HasExpired)
                    {
                        conv.Dispose();
                        _conversationQueue.Dequeue();
                    }

                    foreach (IVoiceSensor sensor in ConfigManager.FindAllComponentsOfType<IVoiceSensor>())
                    {
                        if (_conversationQueue.Count > 0)
                        {
                            sensor.GrammarName = _conversationQueue.Peek().GrammarRuleName;
                        }
                        else
                        {
                            sensor.GrammarName = GrammarUtility.MainGrammarName;
                        }
                    }
                }
                else
                {
                    CheckForInputs();
                }
                CheckForErrors();
                CheckForUpdates();
            }
            catch (Exception e)
            {
                ReportException(e);
                _conversationQueue.Dequeue();
            }
        }

        private void Speak(String speech)
        {
            foreach (IOutputController output in ConfigManager.FindAllComponentsOfType<IOutputController>())
            {
                output.OutputText(speech);
                _lastTimeInteracted = DateTime.Now;
            }
        }

        private void NewInputFound(Object sender, RecognizedNewPhraseEventArgs args)
        {
            try
            {
                bool foundActiveConversation = false;
                foreach (Conversation conv in _conversationQueue)
                {
                    if (conv.TryAddDialog(args.SeenDialog))
                    {
                        foundActiveConversation = true;
                        break;
                    }
                }
                if (!foundActiveConversation)
                {
                    Conversation convo;
                    if (_collection.TryGetConversation(args.SeenDialog, out convo))
                    {
                        _conversationQueue.Enqueue(convo);
                    }
                }
            }
            catch (Exception e)
            {
                ReportException(e);
            }
        }

        private void ReportException(Exception e)
        {
            Console.WriteLine(e.ToString());
            Logger.LogException(ConfigManager, e);
        }

        private void CheckForErrors()
        {
            SAMIUserException exp = ErrorManager.GetInstance().GetNextError();
            if (exp != null)
            {
                _conversationQueue.Enqueue(new ErrorConversation(ConfigManager, exp));
            }
        }
    }
}

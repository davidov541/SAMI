using System;
using System.Collections.Generic;
using System.Timers;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Base class which provides grammars for use with an app.
    /// </summary>
    public sealed class GrammarProvider
    {
        private Func<XmlGrammar> _mainGrammar;
        private Func<IEnumerable<XmlGrammar>> _extraGrammars;
        private readonly Timer _grammarUpdateTimer;

        /// <summary>
        /// Event which is triggered when the grammar needs to be updated.
        /// </summary>
        public event EventHandler GrammarChanged;

        /// <summary>
        /// Triggers a GrammarChanged event.
        /// </summary>
        /// <param name="e">EventArgs instance to trigger this event with. Currenlty, it is ignored.</param>
        private void OnGrammarChanged(EventArgs e)
        {
            if (GrammarChanged != null)
            {
                GrammarChanged(this, e);
            }
        }

        /// <summary>
        /// The main grammar which is listened for when no conversation is active.
        /// </summary>
        internal XmlGrammar MainGrammar
        {
            get
            {
                return _mainGrammar();
            }
        }

        /// <summary>
        /// A list of other grammars that are not the main grammar, but should be 
        /// loaded and will be available for use during secondary dialogs.
        /// </summary>
        internal IEnumerable<XmlGrammar> ExtraGrammars
        {
            get
            {
                return _extraGrammars();
            }
        }

        /// <summary>
        /// Constructor for a grammar provider which provides the main grammar, and a refresh rate.
        /// </summary>
        /// <param name="mainGrammar">Function which returns the main grammar that this GrammarProvider has.</param>
        /// <param name="refreshRate">Indicates how often this grammar should be updated.</param>
        public GrammarProvider(Func<XmlGrammar> mainGrammar, TimeSpan refreshRate)
            : this(mainGrammar, () => new List<XmlGrammar>(), refreshRate)
        {
        }

        /// <summary>
        /// Constructor for a grammar provider which provides the main grammar and any extra grammars.
        /// </summary>
        /// <param name="mainGrammar">The main grammar that this GrammarProvider has.</param>
        /// <param name="extraGrammars">Function which returns any extra grammars that need to be loaded and available, but not used outside of conversations.</param>
        public GrammarProvider(XmlGrammar mainGrammar, params XmlGrammar[] extraGrammars)
            : this(() => mainGrammar, () => extraGrammars)
        {
        }

        private GrammarProvider(Func<XmlGrammar> mainGrammar, Func<IEnumerable<XmlGrammar>> extraGrammars)
        {
            _mainGrammar = mainGrammar;
            _extraGrammars = extraGrammars;
        }

        /// <summary>
        /// Constructor for a grammar provider which provides the main grammar, any extra grammars, and a refresh rate.
        /// </summary>
        /// <param name="mainGrammar">Function which returns the main grammar that this GrammarProvider has.</param>
        /// <param name="extraGrammars">Function which returns any extra grammars that need to be loaded and available, but not used outside of conversations.</param>
        /// <param name="refreshRate">Indicates how often this grammar should be updated.</param>
        public GrammarProvider(Func<XmlGrammar> mainGrammar, Func<IEnumerable<XmlGrammar>> extraGrammars, TimeSpan refreshRate)
            : this(mainGrammar, extraGrammars)
        {
            _grammarUpdateTimer = new Timer(refreshRate.TotalMilliseconds);
            _grammarUpdateTimer.AutoReset = true;
            _grammarUpdateTimer.Elapsed += (sender, args) => OnGrammarChanged(new EventArgs());
            _grammarUpdateTimer.Start();
        }
    }
}

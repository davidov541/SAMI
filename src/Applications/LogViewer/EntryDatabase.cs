using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using SAMI.IOInterfaces;
using SAMI.IOInterfaces.Interfaces.Database;
using SAMI.Logging;

namespace SAMI.Application.LogViewer
{
    internal class EntryDatabase : INotifyPropertyChanged
    {
        private List<LogEntry> _logEntries = new List<LogEntry>();
        public IEnumerable<LogEntry> LogEntries
        {
            get
            {
                return _logEntries.Where(CheckEntryThroughFilter);
            }
            private set
            {
                _logEntries = value.ToList();
                PropertyChanged(this, new PropertyChangedEventArgs("LogEntries"));
            }
        }

        private bool[] _typeFilters;
        public IEnumerable<bool?> TypeFilters
        {
            get
            {
                foreach (bool filter in _typeFilters)
                {
                    yield return filter as bool?;
                }
            }
            set
            {
                int index = 0;
                foreach (bool? filter in value)
                {
                    if (filter.HasValue)
                    {
                        _typeFilters[index] = filter.Value;
                    }
                    index++;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("LogEntries"));
            }
        }

        private String _machineNameFilter = String.Empty;
        public String MachineNameFilter
        {
            get
            {
                return _machineNameFilter;
            }
            set
            {
                _machineNameFilter = value;
                PropertyChanged(this, new PropertyChangedEventArgs("LogEntries"));
            }
        }

        private bool CheckEntryThroughFilter(LogEntry entry)
        {
            bool passesTypeFilter = _typeFilters[(int)entry.Category];
            bool passesMachineNameFilter = String.IsNullOrWhiteSpace(MachineNameFilter) || entry.MachineName.ToLower().Contains(MachineNameFilter.ToLower());
            return passesMachineNameFilter && passesTypeFilter;
        }

        public EntryDatabase()
        {
            _typeFilters = new bool[Enum.GetValues(typeof(LogCategory)).Length];
            for (int i = 0; i < _typeFilters.Length; i++)
            {
                _typeFilters[i] = true;
            }
        }

        public void Refresh()
        {
            RefreshLogEntries();
        }

        private void RefreshLogEntries()
        {
            List<LogEntry> entries = new List<LogEntry>();
            IDatabaseManager man = ConfigurationManager.GetInstance().FindAllComponentsOfType<IDatabaseManager>().FirstOrDefault();
            if(man != null)
            {
                man.StartSession();
                IDataReader reader;
                if (man.TryRunResultQuery("SELECT * FROM dbo.Logs", out reader))
                {
                    while (reader.Read())
                    {
                        entries.Add(new LogEntry((String)reader[1], (DateTime)reader[0], (LogCategory)((byte)reader[2]), (String)reader[3]));
                    }
                    reader.Close();
                }
                man.EndSession();
            }
            LogEntries = entries;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

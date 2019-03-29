using System;
using System.ComponentModel;

namespace SAMI.CompanionApps.Common.SourceModels
{
    public class RoomConfiguration : INotifyPropertyChanged
    {
        public String Name
        {
            get;
            private set;
        }

        public RoomConfiguration(String name)
        {
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

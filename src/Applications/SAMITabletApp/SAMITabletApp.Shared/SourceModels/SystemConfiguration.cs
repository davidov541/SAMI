using System.Collections.Generic;
using System.ComponentModel;

namespace SAMI.CompanionApps.Common.SourceModels
{
    public class SystemConfiguration : INotifyPropertyChanged
    {
        public IEnumerable<RoomConfiguration> Rooms
        {
            get
            {
                yield return new RoomConfiguration("Living Room");
                yield return new RoomConfiguration("Dining Room");
                yield return new RoomConfiguration("Kitchen");
                yield return new RoomConfiguration("Bedroom");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

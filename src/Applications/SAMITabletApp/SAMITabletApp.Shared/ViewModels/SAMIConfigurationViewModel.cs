using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SAMI.CompanionApps.Common.SourceModels;

namespace SAMI.CompanionApps.Common.ViewModels
{
    public class SAMIConfigurationViewModel
    {
        private SystemConfiguration _sourceModel;

        public IEnumerable<IRoomViewModel> Rooms
        {
            get;
            private set;
        }

        public SAMIConfigurationViewModel()
        {
            _sourceModel = new SystemConfiguration();
            _sourceModel.PropertyChanged += sourceModel_PropertyChanged;
            UpdateRooms();
        }

        private void sourceModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Rooms"))
            {
                UpdateRooms();
            }
        }

        private void UpdateRooms()
        {
            Rooms = _sourceModel.Rooms.Select(sm => new RoomViewModel(sm)).Union(new List<IRoomViewModel> { new AddNewRoomViewModel() });
        }
    }
}

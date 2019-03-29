using System;
using System.ComponentModel;
using SAMI.CompanionApps.Common.SourceModels;

namespace SAMI.CompanionApps.Common.ViewModels
{
    public class RoomViewModel : IRoomViewModel
    {
        private RoomConfiguration _sourceModel;

        public String Name
        {
            get;
            private set;
        }

        public String Description
        {
            get
            {
                return "Configure this room!";
            }
        }

        public String ImageUrl
        {
            get
            {
                return "Assets/MediumGray.png";
            }
        }

        public RoomViewModel(RoomConfiguration roomConfig)
        {
            _sourceModel = roomConfig;
            _sourceModel.PropertyChanged += sourceModel_PropertyChanged;
            Name = _sourceModel.Name;
        }

        private void sourceModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
            {
                Name = _sourceModel.Name;
            }
        }
    }
}

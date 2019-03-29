using System;
using System.Collections.Generic;
using System.Text;

namespace SAMI.CompanionApps.Common.ViewModels
{
    public class AddNewRoomViewModel : IRoomViewModel
    {
        public string Name
        {
            get 
            {
                return "New Room";
            }
        }

        public string Description
        {
            get 
            {
                return "Add a new room to your configuration!";
            }
        }

        public string ImageUrl
        {
            get 
            {
                return "Assets/DarkGray.png";
            }
        }
    }
}

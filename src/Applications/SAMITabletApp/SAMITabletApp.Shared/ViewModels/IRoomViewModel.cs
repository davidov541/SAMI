using System;
using System.Collections.Generic;
using System.Text;

namespace SAMI.CompanionApps.Common.ViewModels
{
    public interface IRoomViewModel
    {
        String Name
        {
            get;
        }

        String Description
        {
            get;
        }

        String ImageUrl
        {
            get;
        }
    }
}

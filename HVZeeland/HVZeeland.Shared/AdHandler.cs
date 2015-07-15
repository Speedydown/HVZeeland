using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace HVZeeland
{
    public static class AdHandler
    {
        public static bool DisplayAD()
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;


            return true;
        }
    }
}

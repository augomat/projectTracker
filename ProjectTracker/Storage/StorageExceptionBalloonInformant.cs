using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker
{
    class StorageExceptionBalloonInformant
    {
        public delegate void ShowNotification(string title, string text);
        private ShowNotification showBalloon;

        public StorageExceptionBalloonInformant(ShowNotification notificationDelegate)
        {
            showBalloon = notificationDelegate;
        }

        public void handleStorageException(object sender, Exception ex)
        {
            showBalloon("Storage Exception", ex.ToString());
        }
    }
}

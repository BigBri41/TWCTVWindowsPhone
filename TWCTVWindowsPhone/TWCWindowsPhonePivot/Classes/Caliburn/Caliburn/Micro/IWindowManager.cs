namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IWindowManager
    {
        void ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null);
        void ShowNotification(object rootModel, int durationInMilliseconds, object context = null, IDictionary<string, object> settings = null);
        void ShowPopup(object rootModel, object context = null, IDictionary<string, object> settings = null);
    }
}


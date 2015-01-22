using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public abstract class ObservableSettingsObject<T> : ObservableObject<T>
    {

        public override void Render()
        {
            Type t = typeof(T);
            SettingsWindowModel mw = MoustacheClient.Instance.Settings.Model;
            mw.CurrentPageViewModel = mw.PageViewModelMap[t.Name];
        }


        public static T Instance
        {
            get
            {
                Type t = typeof(T);
                SettingsWindowModel mw = MoustacheClient.Instance.Settings.Model;
                return (T)mw.PageViewModelMap[t.Name];
            }
        }
    }
}

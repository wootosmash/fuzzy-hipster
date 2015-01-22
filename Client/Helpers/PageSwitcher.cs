using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    class PageSwitcher<T>
    {
        public static T Instance
        {
            get
            {
                Type t = typeof(T);
                MoustacheClientModel mw = MoustacheClient.Instance.Model;
                return (T)mw.PageViewModelMap[t.Name];
            }
        }

        public void Render()
        {
            Type t = typeof(T);
            MoustacheClientModel mw = MoustacheClient.Instance.Model;
            mw.CurrentPageViewModel = mw.PageViewModelMap[t.Name];
        }
    }
}

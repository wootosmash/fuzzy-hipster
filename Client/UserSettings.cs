using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Client
{
    class UserSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue(null)]
        public List<KeyValuePair<string, string>> Keys
        {
            get
            {
                
                return ((List<KeyValuePair<string, string>>)this["Keys"]);
            }
            set
            {
                this["Keys"]  = value;                
            }
        }
    }
}

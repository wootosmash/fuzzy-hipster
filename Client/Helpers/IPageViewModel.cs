﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IPageViewModel
    {
        string Name { get; }
        bool IsActive { get; set; }
        
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoptCompanions.Settings
{
    internal interface ISettingsProvider
    {
        int RelationshipMinimum { get; set; }
        float AgreeChance { get; set; }
        bool Debug { get; set; }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

using TaleWorlds.Library;

namespace AdoptCompanions.Settings
{
    internal class ACSettings : ISettingsProvider
    {
        public int RelationshipMinimum { get => -100; set => throw new NotImplementedException(); }
        public float AgreeChance { get => 100; set => throw new NotImplementedException(); }
        public bool Debug { get => true; set => throw new NotImplementedException(); }

    }
}

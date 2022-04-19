using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoptCompanions.Settings
{
    internal class ASSettings : AttributeGlobalSettings<ASSettings>
    {

        public override string Id => "Assassination";

        public override string DisplayName => "Assassination";

        public override string FolderName => "AssassinationSettings";

        public override string FormatType => "json";

        //dev configs
        public bool Debug { get; set; } = true;

        //Settings for Type of people that can be adopted
        //based on type
        [SettingPropertyBoolAttribute("Can assassinate?", HintText = "default is enabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Who can be assassinated?", GroupOrder = 1)]
        public bool canAssassinate { get; set; } = true;

        [SettingPropertyBoolAttribute("Can assassinate lords?", HintText = "default is enabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Who can be assassinated?", GroupOrder = 1)]
        public bool canAssassinateLords { get; set; } = true;

        [SettingPropertyBoolAttribute("Can assassinate faction leaders?", HintText = "default is enabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Who can be assassinated?", GroupOrder = 1)]
        public bool canAssassinateKings { get; set; } = true;

        [SettingPropertyBoolAttribute("Can assassinate Notables?", HintText = "default is enabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Who can be assassinated?", GroupOrder = 1)]
        public bool canAssassinateNotables { get; set; } = true;

        [SettingPropertyBoolAttribute("Can assassinate family?", HintText = "default is enabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Who can be assassinated?", GroupOrder = 1)]
        public bool canAssassinateFamily { get; set; } = true;

        [SettingPropertyBoolAttribute("Can assassinate Other factions?", HintText = "default is enabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Who can be assassinated?", GroupOrder = 1)]
        public bool canAssassinateOtherFactions { get; set; } = true;

        [SettingPropertyBoolAttribute("Can assassinate at war?", HintText = "default is enabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Who can be assassinated?", GroupOrder = 1)]
        public bool canAssassinateAtWar { get; set; } = true;

        [SettingPropertyBoolAttribute("Can assassinate children?", HintText = "default is enabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Who can be assassinated?", GroupOrder = 1)]
        public bool canAssassinateChildren { get; set; } = true;

        [SettingPropertyIntegerAttribute("Cost of Assassin?", 0, 9999999, HintText = "The cost of assassination in gold. default is 150,000.", Order = 20, RequireRestart = false)]
        [SettingPropertyGroup("Assassination cost and effects?", GroupOrder = 3)]
        public int assassinatationCost { get; set; } = 150000;
    }
}

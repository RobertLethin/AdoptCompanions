using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

using TaleWorlds.Library;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Settings.Base.Global;

namespace AdoptCompanions.Settings
{
    internal class ACSettings : AttributeGlobalSettings<ACSettings>
    {

        public override string Id => "AdoptCompanions";

        public override string DisplayName => "Adopt Companions";

        public virtual string FolderName => "Settings";

        public virtual string FormatType => "json2";

        //dev configs
        public bool Debug { get; set; } = true;

        //Settings for Type of people that can be adopted
        //based on type
        [SettingPropertyBoolAttribute("Can adopt Anyone?", HintText = "Enabling this will allow you to adopt anyone and will enable all other flags for who you can adopt. default is disabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("Type of people that can be adopted")]
        public bool canAdoptAnyone { get; set; } = true;

        [SettingPropertyBoolAttribute("Can adopt Companions?", HintText = "default is enabled.", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("Type of people that can be adopted")]
        public bool canAdoptCompanions { get; set; } = true;

        [SettingPropertyBoolAttribute("Can adopt Faction leaders (Kings/Queens)?", HintText = "default is disabled.", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("Type of people that can be adopted")]
        public bool canAdoptKings { get; set; } = false;

        [SettingPropertyBoolAttribute("Can adopt Lords from the same faction only?", HintText = "If you enable lords from any faction this will also be enabled. default is disabled. ", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("Type of people that can be adopted")]
        public bool canAdoptLordsSameFaction { get; set; } = false;

        [SettingPropertyBoolAttribute("Can adopt Lords from any faction?", HintText = "default is disabled.", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("Type of people that can be adopted")]
        public bool canAdoptLordsAnyFaction { get; set; } = false;

        [SettingPropertyBoolAttribute("Can adopt Notables (merchants and such) from the same faction only?", HintText = "If you enable notables from any faction this will also be enabled. default is disabled.", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("Type of people that can be adopted")]
        public bool canAdoptNotablesSameFaction { get; set; } = false;

        [SettingPropertyBoolAttribute("Can adopt Notables (merchants and such) from any faction?", HintText = "default is disabled.", Order = 6, RequireRestart = false)]
        [SettingPropertyGroup("Type of people that can be adopted")]
        public bool canAdoptNotablesAnyFaction { get; set; } = false;

        //        [SettingPropertyBoolAttribute("Troops", HintText = "default is disabled.", Order = 0, RequireRestart = false)]
        //        [SettingPropertyGroup("Type of people that can be adopted")]
        //        public bool canAdoptTroops { get; set; } = false;

        //        [SettingPropertyBoolAttribute("Other people not covered from above.", HintText = "default is disabled. Enabling this will enable adoption on everyone.", Order = 0, RequireRestart = false)]
        //        [SettingPropertyGroup("Type of people that can be adopted")]
        //        public bool canAdoptOtherType { get; set; } = false;


        //based on conditions
        [SettingPropertyBoolAttribute("Can adopt from factions you are at war with?", HintText = "default is disabled.", Order = 7, RequireRestart = false)]
        [SettingPropertyGroup("Diplomacy Conditions for Adpotion")]
        public bool canAdoptAtWar { get; set; } = false;

        [SettingPropertyBoolAttribute("Can adopt prisoners?", HintText = "This only applies to lords/people you can speak to not normal troops. default is disabled.", Order = 8, RequireRestart = false)]
        [SettingPropertyGroup("Diplomacy Conditions for Adpotion")]
        public bool canAdoptPrisoners { get; set; } = false;

        [SettingPropertyBoolAttribute("Can adopt enimies?", HintText = "Can adopt people that view you as enemies. default is disabled.", Order = 8, RequireRestart = false)]
        [SettingPropertyGroup("Diplomacy Conditions for Adpotion")]
        public bool canAdoptEnimies { get; set; } = false;
        public bool canAdoptOtherFactions { get; set; } = true;


        //Settings for if it pass/fails
        //RNG chance for if they agree after meeting requirements 
        [SettingPropertyIntegerAttribute("RNG percent chance they will agree to your request", 0, 100, HintText = "This is an RNG percent chance someone who meets all other conditons will agree to being adpoted. default is 100% chance", Order = 9, RequireRestart = false)]
        [SettingPropertyGroup("Standard Adoption requirements")]
        public int AgreeChance { get; set; } = 100;

        //minimum relationship before able to adopt
        [SettingPropertyIntegerAttribute("Minimum relationship required", -100, 100, HintText = "This is the minimum relationship required for someone to be adpotable. default is 75", Order = 10, RequireRestart = false)]
        [SettingPropertyGroup("Standard Adoption requirements")]
        public int RelationshipMinimum { get; set; } = 75;

        //minimum relationship before able to adopt
        [SettingPropertyIntegerAttribute("Minimum days you have known someone required. Note: ONLY WORKS WITH NEW SAVES", 0, 10000, HintText = "This is the minimum amount of days since you met someone before you can adopt them. Note default is 0 days", Order = 11, RequireRestart = false)]
        [SettingPropertyGroup("Standard Adoption requirements")]
        public int TimeKnownMinimum { get; set; } = 0;

        //minimum relationship before able to adopt
        [SettingPropertyIntegerAttribute("Cost of Adoption (gold required to give to adoptee)", 0, 1000000, HintText = "This is the amount of gold you have to pay someone for them to agree to being adopted. default is 0 denars", Order = 11, RequireRestart = false)]
        [SettingPropertyGroup("Standard Adoption requirements")]
        public int GoldCostOfAdoption { get; set; } = 0;

        //minimum relationship before able to adopt
        [SettingPropertyBoolAttribute("Can already have living parents?", HintText = "If you are allowed to adopt someone that already has living parents. default is true.", Order = 11, RequireRestart = false)]
        [SettingPropertyGroup("Standard Adoption requirements")]
        public bool CanAdoptHasLivingParents { get; set; } = true;





        //Setting for what happens if pass
        //Relationship gain on adoption
        [SettingPropertyIntegerAttribute("Relationship change on sucessful adoption", -100, 100, HintText = "The relation gain/loss you recieve when you adopt someone. default is +30", Order = 10, RequireRestart = false)]
        [SettingPropertyGroup("Effect of adoption")]
        public int RelationshipGainPass { get; set; } = 30;


        //Setting for what happens if fails
        //Relationship gain on fail
        [SettingPropertyIntegerAttribute("Relationship change on failed adoption", -100, 100, HintText = "The relation gain/loss you recieve when you try to adopt someone but fail. default is +10", Order = 10, RequireRestart = false)]
        [SettingPropertyGroup("Effect of adoption")]
        public int RelationshipGainFail { get; set; } = 10;

      
    }
}

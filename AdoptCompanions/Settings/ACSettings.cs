﻿using System;
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

        public override string FolderName => "AdpotCompanionsSettings";

        public override string FormatType => "json";

        //dev configs
        public bool Debug { get; set; } = true;

        //Settings for Type of people that can be adopted
        //based on type
        [SettingPropertyBoolAttribute("{=STR_SET_00018}Can adpot from different kingdoms/factions?", HintText = "{=STR_SET_00016}default is disabled.", Order = 0, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00001}Diplomacy Conditions for Adpotion", GroupOrder = 1)]
        public bool canAdoptDifferentFactions { get; set; } = false;
        //based on conditions
        [SettingPropertyBoolAttribute("{=STR_SET_00019}Can adopt from factions you are at war with?", HintText = "{=STR_SET_00016}default is disabled.", Order = 1, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00001}Diplomacy Conditions for Adpotion", GroupOrder = 1)]
        public bool canAdoptAtWar { get; set; } = false;

        [SettingPropertyBoolAttribute("{=STR_SET_00020}Can adopt prisoners?", HintText = "{=STR_SET_00021}This only applies to lords/people you can speak to not normal troops. default is enabled.", Order = 2, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00001}Diplomacy Conditions for Adpotion", GroupOrder = 1)]
        public bool canAdoptPrisoners { get; set; } = true;



        //Settings for Type of people that can be adopted
        //based on type
        [SettingPropertyBoolAttribute("{=STR_SET_00022}Can adopt Companions?", HintText = "{=STR_SET_00017}default is enabled.", Order = 3, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00002}Type of people that can be adopted", GroupOrder = 2)]
        public bool canAdoptCompanions { get; set; } = true;

        [SettingPropertyBoolAttribute("{=STR_SET_00023}Can adopt Faction leaders (Kings/Queens)?", HintText = "{=STR_SET_00024}Note: The leader will join your clan and a new leader will be assigned to their old faction. default is disabled.", Order = 4, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00002}Type of people that can be adopted", GroupOrder = 2)]
        public bool canAdoptKings { get; set; } = false;

        [SettingPropertyBoolAttribute("{=STR_SET_00025}Can adopt Lords?", HintText = "{=STR_SET_00016}default is disabled. ", Order = 5, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00002}Type of people that can be adopted", GroupOrder = 2)]
        public bool canAdoptLords { get; set; } = false;

        [SettingPropertyBoolAttribute("{=STR_SET_00026}Can adopt Notables (merchants and such)?", HintText = "{=STR_SET_00016}default is disabled.", Order = 6, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00002}Type of people that can be adopted", GroupOrder = 2)]
        public bool canAdoptNotables { get; set; } = false;

        [SettingPropertyBoolAttribute("{=STR_SET_00027}Can adopt Children?", HintText = "{=STR_SET_00016}default is disabled.", Order = 7, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00002}Type of people that can be adopted", GroupOrder = 2)]
        public bool canAdoptChildren { get; set; } = false;

        //Settings for if it pass/fails
        //minimum relationship before able to adopt
        [SettingPropertyIntegerAttribute("{=STR_SET_00028}Minimum relationship required", -100, 100, HintText = "{=STR_SET_00029}This is the minimum relationship required for someone to be adpotable. default is 75", Order = 10, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00003}Standard Adoption requirements", GroupOrder = 3)]
        public int RelationshipMinimum { get; set; } = 60;

        //RNG chance for if they agree after meeting requirements 
/*        [SettingPropertyIntegerAttribute("RNG percent chance they will agree to your request", 0, 100, HintText = "This is an RNG percent chance someone who meets all other conditons will agree to being adpoted. default is 100% chance", Order = 11, RequireRestart = false)]
        [SettingPropertyGroup("Standard Adoption requirements")]
        public int AgreeChance { get; set; } = 100;
*/

/*        //gold cost to adopt
        [SettingPropertyIntegerAttribute("Cost of Adoption (gold required to give to adoptee)", 0, 1000000, HintText = "This is the amount of gold you have to pay someone for them to agree to being adopted. default is 0 denars", Order = 12, RequireRestart = false)]
        [SettingPropertyGroup("Standard Adoption requirements", GroupOrder = 3)]
        public int GoldCostOfAdoption { get; set; } = 0;
*/
/*        //Can adopt if already have living parents
        [SettingPropertyBoolAttribute("Can already have living parents?", HintText = "If you are allowed to adopt someone that already has living parents. default is true.", Order = 13, RequireRestart = false)]
        [SettingPropertyGroup("Standard Adoption requirements")]
        public bool CanAdoptHasLivingParents { get; set; } = true;
*/


        //Setting for what happens if pass
        //Relationship gain on adoption
        [SettingPropertyIntegerAttribute("{=STR_SET_00030}Relationship change on sucessful adoption", -100, 100, HintText = "{=STR_SET_00031}The relation gain/loss you recieve when you adopt someone. default is +30", Order = 20, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00004}Effects of adoption", GroupOrder = 4)]
        public int RelationshipGainPass { get; set; } = 30;


        //Setting for what happens if fails
        //Relationship gain on fail
        [SettingPropertyIntegerAttribute("{=STR_SET_00032}Relationship change on failed adoption", -100, 100, HintText = "{=STR_SET_00033}The relation gain/loss you recieve when you try to adopt someone but fail. default is -10", Order = 21, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00004}Effects of adoption", GroupOrder = 4)]
        public int RelationshipGainFail { get; set; } = -10;

        //Unadopt settings
        [SettingPropertyBoolAttribute("{=STR_SET_00034}Can disown children?", HintText = "{=STR_SET_00017}default is enabled.", Order = 31, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00005}Remove from Family", GroupOrder = 5)]
        public bool canUnAdoptChildren { get; set; } = true;

        [SettingPropertyBoolAttribute("{=STR_SET_00035}Can disown siblings?", HintText = "{=STR_SET_00017}default is enabled.", Order = 32, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00005}Remove from Family", GroupOrder = 5)]
        public bool canUnAdoptSiblings { get; set; } = true;

        [SettingPropertyBoolAttribute("{=STR_SET_00036}Can divorce spouse?", HintText = "{=STR_SET_00016}default is disabled.", Order = 33, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00005}Remove from Family", GroupOrder = 5)]
        public bool canUnAdoptSpouse { get; set; } = false;

        [SettingPropertyBoolAttribute("{=STR_SET_00037}Can disown parents?", HintText = "{=STR_SET_00016}default is disabled.", Order = 34, RequireRestart = false)]
        [SettingPropertyGroup("{=STR_SET_00005}Remove from Family", GroupOrder = 5)]
        public bool canUnAdoptParents { get; set; } = false;

    }
}

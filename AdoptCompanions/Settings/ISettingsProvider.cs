using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoptCompanions.Settings
{
    internal interface ISettingsProvider
    {
        public bool Debug { get; set; }


        //Settings for who can be adopted
        //based on type
        public bool canAdoptCompanions { get; set; }
        public bool canAdoptKings { get; set; }
        public bool canAdoptLordsSameFaction { get; set; }
        public bool canAdoptLordsAnyFaction { get; set; }
        public bool canAdoptNotablesSameFaction { get; set; }
        public bool canAdoptNotablesAnyFaction { get; set; }
        public bool canAdoptTroops { get; set; }
        public bool canAdoptOtherType { get; set; }


        //based on conditions
        public bool canAdoptAtWar { get; set; }
        public bool canAdoptPrisoner { get; set; }
        public bool canAdoptOtherFactions { get; set; }
        public bool canAdopt { get; set; }





        //Settings for if it pass/fails
        //RNG chance for if they agree after meeting requirements 
        public int AgreeChance { get; set; }

        //minimum relationship before able to adopt
        public int RelationshipMinimum { get; set; }


        //Setting for what happens if pass
        //Relationship gain on adoption
        public int RelationshipGainPass { get; set; }


        //Setting for what happens if fails
        //Relationship gain on fail
        public int RelationshipGainFail { get; set; }
    }
}

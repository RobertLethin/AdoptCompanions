using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoptCompanions.common
{
    internal class AdoptConstants
    {
		internal const String MOD_NAME = "Adopt Companions";
		
        //consts for canAdopt checks
        internal const int FAIL_FAMILY = -1;
        internal const int FAIL_DIFFERENT_FACTION = -2;
        internal const int FAIL_AT_WAR = -3;
        internal const int FAIL_PRISONER = -4;
        internal const int FAIL_COMPANION = -5;
        internal const int FAIL_FACTION_LEADER = -6;
        internal const int FAIL_LORD = -7;
        internal const int FAIL_NOTABLE = -8;
        internal const int FAIL_CHILDREN = -9;
        internal const int FAIL_DEAD = -10;
        internal const int FAIL_OTHER = -100;

        internal const int PASS_FAMILY = 1;
        internal const int PASS_DIFFERENT_FACTION = 2;
        internal const int PASS_AT_WAR = 3;
        internal const int PASS_PRISONER = 4;
        internal const int PASS_COMPANION = 5;
        internal const int PASS_FACTION_LEADER = 6;
        internal const int PASS_LORD = 7;
        internal const int PASS_NOTABLE = 8;
        internal const int PASS_CHILDREN = 9;
        internal const int PASS_DEAD = 10;
        internal const int PASS_OTHER = 100;


        //for relationshipcheck
        internal const int FAIL_RELATIONSHIP = -1;
        internal const int PASS_RELATIONSHIP = 1;

        //family/relationship
        internal const int FAMILY_PARENT = 1;
        internal const int FAMILY_SPOUSE = 2;
        internal const int FAMILY_SIBLING = 3;
        internal const int FAMILY_CHILD = 4;
        internal const int NOT_FAMILY = -1;

        //adoption types
        internal const int TYPE_ID_OTHER = 0;
        internal const int TYPE_ID_SIBLING = 1;
        internal const int TYPE_ID_CHILD = 2;

        internal const String TYPE_STR_OTHER = "other";
        internal const String TYPE_STR_SIBLING = "sibling";
        internal const String TYPE_STR_CHILD = "child";

        internal const String GENDER_SIBLING_MALE = "brother";
        internal const String GENDER_SIBLING_FEMALE = "sister";
        internal const String GENDER_CHILD_MALE = "son";
        internal const String GENDER_CHILD_FEMALE = "daughter";
    }
}

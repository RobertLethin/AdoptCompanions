using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoptCompanions.common
{
    internal class AdoptConstants
    {
        //consts for canAdopt checks
        public const int FAIL_FAMILY = -1;
        public const int FAIL_DIFFERENT_FACTION = -2;
        public const int FAIL_AT_WAR = -3;
        public const int FAIL_PRISONER = -4;
        public const int FAIL_COMPANION = -5;
        public const int FAIL_FACTION_LEADER = -6;
        public const int FAIL_LORD = -7;
        public const int FAIL_NOTABLE = -8;
        public const int FAIL_CHILDREN = -9;
        public const int FAIL_DEAD = -10;
        public const int FAIL_OTHER = -100;

        public const int PASS_FAMILY = 1;
        public const int PASS_DIFFERENT_FACTION = 2;
        public const int PASS_AT_WAR = 3;
        public const int PASS_PRISONER = 4;
        public const int PASS_COMPANION = 5;
        public const int PASS_FACTION_LEADER = 6;
        public const int PASS_LORD = 7;
        public const int PASS_NOTABLE = 8;
        public const int PASS_CHILDREN = 9;
        public const int PASS_DEAD = 10;
        public const int PASS_OTHER = 100;


        //for relationshipcheck
        public const int FAIL_RELATIONSHIP = -1;
        public const int PASS_RELATIONSHIP = 1;

        //adoption types
        public const int TYPE_ID_OTHER = 0;
        public const int TYPE_ID_SIBLING = 1;
        public const int TYPE_ID_CHILD = 2;

        public const String TYPE_STR_OTHER = "other";
        public const String TYPE_STR_SIBLING = "sibling";
        public const String TYPE_STR_CHILD = "child";

        public const String GENDER_SIBLING_MALE = "brother";
        public const String GENDER_SIBLING_FEMALE = "sister";
        public const String GENDER_CHILD_MALE = "son";
        public const String GENDER_CHILD_FEMALE = "daughter";
    }
}

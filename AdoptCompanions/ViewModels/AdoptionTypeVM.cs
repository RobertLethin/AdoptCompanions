using AdoptCompanions.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;

namespace AdoptCompanions.ViewModels
{
    internal class AdoptionTypeVM
    {
        public String typeStr { get; set; }

        public int typeId { get; set; }

        public bool isSelected { get; set; }

        public String genderedType { get; set; }

        public AdoptionTypeVM(String typeStr)
        {
            this.typeStr = typeStr;

            if (typeStr.Equals(AdoptConstants.TYPE_STR_SIBLING))
            {
                typeId = AdoptConstants.TYPE_ID_SIBLING;
            } else if (typeStr.Equals(AdoptConstants.TYPE_STR_CHILD))
            {
                typeId = AdoptConstants.TYPE_ID_CHILD;
            } else
            {
                typeId = AdoptConstants.TYPE_ID_OTHER;
            }

            isSelected = false;
        }

        public AdoptionTypeVM(int typeId)
        {
            this.typeId = typeId;

            if (typeId == AdoptConstants.TYPE_ID_SIBLING)
            {
                typeStr = AdoptConstants.TYPE_STR_SIBLING;
            }
            else if (typeId == AdoptConstants.TYPE_ID_CHILD)
            {
                typeStr = AdoptConstants.TYPE_STR_CHILD;
            }
            else
            {
                typeStr = AdoptConstants.TYPE_STR_OTHER;
            }

            isSelected = false;
        }

        public AdoptionTypeVM(String typeStr, int typeId)
        {
            this.typeStr = typeStr;

            this.typeId = typeId;  

            isSelected = false;
        }

        public AdoptionTypeVM(String typeStr, int typeId, bool isSelected)
        {
            this.typeStr = typeStr;

            this.typeId = typeId;

            this.isSelected = isSelected;
        }

        public void setGenderedType(Hero hero)
        {
            if (hero.IsFemale)
            {
                if (typeId == AdoptConstants.TYPE_ID_SIBLING)
                {
                    this.genderedType = AdoptConstants.GENDER_SIBLING_FEMALE;
                } else if (typeId == AdoptConstants.TYPE_ID_CHILD)
                {
                    this.genderedType = AdoptConstants.GENDER_CHILD_FEMALE;
                } else
                {
                    this.genderedType = "OTHER";
                }
            } else {
                if (typeId == AdoptConstants.TYPE_ID_SIBLING)
                {
                    this.genderedType = AdoptConstants.GENDER_SIBLING_MALE;
                }
                else if (typeId == AdoptConstants.TYPE_ID_CHILD)
                {
                    this.genderedType = AdoptConstants.GENDER_CHILD_MALE;
                }
                else
                {
                    this.genderedType = "OTHER";
                }
            }
        }
    }
}

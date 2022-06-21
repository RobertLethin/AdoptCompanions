using AdoptCompanions.Actions;
using AdoptCompanions.common;
using AdoptCompanions.ViewModels;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement.Categories;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace AdoptCompanions.Patches
{
    //[HarmonyPatch(typeof(ClanMembersVM), "Refresh")]
    /*
    internal class ClanLordItemVMPatch
    {
        public static bool Prefix(ClanMembersVM __instance)
        {
            
            foreach (ClanLordItemVM lordItem in ((ClanMembersVM)__instance).Family)
            {
                if ( ACHelper.isFamily(Hero.MainHero, lordItem.GetHero()) < 1)
                {
                    //neeed to update
                    //__instance.IsFamilyMember = false;
                    __instance.Family.Remove(lordItem);
                }
            }
            return true;
        }
    }
    */
}

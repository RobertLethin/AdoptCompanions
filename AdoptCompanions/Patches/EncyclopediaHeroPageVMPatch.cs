using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdoptCompanions.ViewModels;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.Core.ViewModelCollection;

namespace AdoptCompanions.Patches { 

    [HarmonyPatch(typeof(EncyclopediaHeroPageVM), "Refresh")]
    public class EncyclopediaHeroPageVMPatch
    {
        public static bool Prefix(EncyclopediaHeroPageVM __instance)
        {
            Hero hero = ((EncyclopediaPageVM)__instance).Obj as Hero;
            if (ACHelper.canAdopt(hero) > 0)
                __instance.HeroCharacter = (HeroViewModel)new HeroViewModelAC(hero, (CharacterViewModel.StanceTypes)1);
            return true;
        }
    }
}

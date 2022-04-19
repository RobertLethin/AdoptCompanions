using AdoptCompanions.Settings;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;

namespace AdoptCompanions.Actions
{
    internal class AssassinateAction
    {
        public static void Assassinate(Hero hero)
        {
            KillCharacterAction.ApplyByMurder(hero);
            Hero.MainHero.ChangeHeroGold(0 - GlobalSettings<ASSettings>.Instance.assassinatationCost);
        }
    }
}
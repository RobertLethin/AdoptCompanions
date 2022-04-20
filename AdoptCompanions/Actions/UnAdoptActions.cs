using AdoptCompanions.common;
using AdoptCompanions.Settings;
using MCM.Abstractions.Settings.Base.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;

namespace AdoptCompanions.Actions
{
    internal class UnAdoptActions
    {
        public enum UnAdoptActionDetail
        {
            None,
            Companion,
            Independent
        }

        internal static void UnAdoptAction(Hero hero, UnAdoptActionDetail detail = UnAdoptActionDetail.None)
        {
            int relation = ACHelper.isFamily(Hero.MainHero, hero);
            bool convertToCompanion = true;

            if (relation == AdoptConstants.FAMILY_SIBLING)
            {
                if (hero.Father.Children.Contains(hero))
                    hero.Father.Children.Remove(hero);

                if (hero.Mother.Children.Contains(hero))
                    hero.Mother.Children.Remove(hero);

                hero.Mother = null;
                hero.Father = null;
            }
            else if (relation == AdoptConstants.FAMILY_CHILD)
            {
                hero.Mother = null;
                hero.Father = null;
                
                if(Hero.MainHero.Children.Contains(hero))
                    Hero.MainHero.Children.Remove(hero);


            } 
            else if (relation == AdoptConstants.FAMILY_SPOUSE)
            {
                //convertToCompanion = false;
                hero.Spouse = null;
                Hero.MainHero.Spouse = null;
            }
            else if (relation == AdoptConstants.FAMILY_PARENT)
            {
                if(Hero.MainHero.Mother == hero)
                    Hero.MainHero.Mother = null;
                if (Hero.MainHero.Father == hero)
                    Hero.MainHero.Father = null;

                if (hero.Children.Contains(Hero.MainHero))
                    hero.Children.Remove(Hero.MainHero);

            }



            if (hero.Clan == Hero.MainHero.Clan
                && ( detail == UnAdoptActionDetail.None
                || detail == UnAdoptActionDetail.Companion))
            {
                hero.Spouse = null;
                if (hero.Occupation != Occupation.Wanderer)
                {
                    hero.SetNewOccupation(Occupation.Wanderer);
                }
                /*                
                                if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.Owner == hero)
                                {
                                    DisbandPartyAction(hero);
                                }
                */
                hero.CompanionOf = Hero.MainHero.Clan;
                hero.IsNoble = false; //not sure if I should do this or not

                ACHelper.Print("Adopt Companions: Successfully made " + hero.Name + " into a companion.");
            }
            else if (hero.Clan == Hero.MainHero.Clan && !convertToCompanion)
            {
                //hero.Clan = GetDisownedClan();
            }
            else if (detail == UnAdoptActionDetail.Companion)
            {
                //TODO: convert all to companions
            }
            else if (detail == UnAdoptActionDetail.Independent)
            {
                //TODO: convert all to no factionless clan
            }

            ACHelper.Print("Adopt Companions: Successfully kicked " + hero.Name + " out of your family.");

        }

        private static Clan GetDisownedClan()
        {
            foreach (Clan clan in Campaign.Current.Clans)
            {
                if (clan.Name.Equals("The Disowned"))
                {
                    return clan;
                }
            }

            Clan disownedClan = new Clan();
            disownedClan.ChangeClanName(new TextObject("The Disowned") );
            return disownedClan;
        }

        public static void UnAdoptActionCompanion(Hero hero)
        {
            
        }

        public static void UnAdoptActionIndependent(Hero hero)
        {

        }

        internal static bool CanUnAdopt(Hero hero)
        {
            int relation = ACHelper.isFamily(Hero.MainHero, hero);

            if (relation == AdoptConstants.FAMILY_SIBLING && GlobalSettings<ACSettings>.Instance.canUnAdoptSiblings)
            {
                return true;
            } 
            else if (relation == AdoptConstants.FAMILY_CHILD && GlobalSettings<ACSettings>.Instance.canUnAdoptChildren)
            {
                return true;
            }
            else if (relation == AdoptConstants.FAMILY_SPOUSE && GlobalSettings<ACSettings>.Instance.canUnAdoptSpouse)
            {
                return true;
            }
            else if (relation == AdoptConstants.FAMILY_PARENT && GlobalSettings<ACSettings>.Instance.canUnAdoptParents)
            {
                return true;
            }

            return false;
        }


    }
}

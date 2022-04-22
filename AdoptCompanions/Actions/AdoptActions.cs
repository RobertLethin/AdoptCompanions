using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using AdoptCompanions.Settings;
using AdoptCompanions.common;
using MCM.Abstractions.Settings.Base.Global;
using AdoptCompanions.ViewModels;
using TaleWorlds.Localization;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;

namespace AdoptCompanions.Actions
{
    internal class AdoptActions
    {
        public static int AdoptAction(Hero hero, AdoptionTypeVM adoptionType)
        {
            if (ACHelper.canAdopt(hero) < 0 && ACHelper.checkRelationship(hero) == AdoptConstants.FAIL_RELATIONSHIP)
            {
                return -1;
            }
            if (adoptionType.typeId == AdoptConstants.TYPE_ID_SIBLING)
            {
                return AdoptSiblingAction(hero);
            }
            else if (adoptionType.typeId == AdoptConstants.TYPE_ID_CHILD)
            {
                return AdoptChildAction(hero);
            }

            return -1;
        }

        //Adopt hero as players sibling
        private static int AdoptSiblingAction(Hero hero)
        {

            //perform logic for updating if clan leader or faction ruler
            //and logic for reassigning family clan if needed (young children and spouse)
            performHeroClanUpdates(ref hero);
            performHeroFamilyUpdates(ref hero);

            //need to be lord to be family
            if (hero.Occupation != Occupation.Lord)
            {
                hero.SetNewOccupation(Occupation.Lord);
            }

            //remove from being prisoner
            if (hero.IsMercenary)
            {
                hero.IsMercenary = false;
            }

            if (hero.GovernorOf != null)
            {
                ChangeGovernorAction.Apply(hero.GovernorOf, null);
            }


            hero.IsNoble = true;
            hero.HasMet = true;
            hero.CompanionOf = null;

            changeAdoptedHeroParty(hero);

            hero.Clan = Hero.MainHero.Clan;
            //set same parents as player (which effectively makes them siblings)
            hero.Father = Hero.MainHero.Father;
            hero.Mother = Hero.MainHero.Mother;

            //Increase relationship by 30 (old version hard set it to 100)
            hero.SetPersonalRelation(Hero.MainHero, (hero.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));

            if (hero.IsPrisoner)
            {
                releaseAdoptedPrisoner(hero);
            }



            OnHeroAdopted(Hero.MainHero, hero, true);

            ACHelper.Print("Successfully adopted " + hero.Name + " as your new sibling.");
            return 0;
        }

        //Adopt hero as player's child
        private static int AdoptChildAction(Hero hero)
        {

            //perform logic for updating if clan leader or faction ruler
            //and logic for reassigning family clan if needed (young children and spouse)
            performHeroClanUpdates(ref hero);
            performHeroFamilyUpdates(ref hero);

            if (hero.Occupation != Occupation.Lord)
            {
                hero.SetNewOccupation(Occupation.Lord);
                //AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
            }

            if (hero.IsMercenary)
            {
                hero.IsMercenary = false;
            }

            if (hero.GovernorOf != null)
            {
                ChangeGovernorAction.Apply(hero.GovernorOf, null);
            }

            hero.IsNoble = true;
            hero.Clan = Hero.MainHero.Clan;
            hero.HasMet = true;
            hero.CompanionOf = null;
            hero.Father = null;
            hero.Mother = null;

            //First set spouse as parent
            //Should be first in case homosexual marriage causes issues to ensure player always gets set as parent at minimum
            //Only works for wives?
            if (Hero.MainHero.Spouse != null)
            {
                Hero spouse = Hero.MainHero.Spouse;

                if (spouse.IsFemale)
                {
                    hero.Mother = spouse;
                }
                else
                {
                    hero.Father = spouse;
                }

                hero.SetPersonalRelation(spouse, (hero.GetRelation(spouse) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));
                spouse.SetPersonalRelation(hero, (spouse.GetRelation(hero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));
            }
            else
            {
                //above might not work properly for female heros or homosexual relationships so check all heros if they are spouse of player
                foreach (Hero checkHero in Campaign.Current.AliveHeroes)
                {
                    if (checkHero.Spouse != null)
                    {
                        if (checkHero.Spouse == Hero.MainHero)
                        {
                            if (checkHero.IsFemale)
                            {
                                hero.Mother = checkHero;
                            }
                            else
                            {
                                hero.Father = checkHero;
                            }

                            hero.SetPersonalRelation(checkHero, (hero.GetRelation(checkHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));
                            checkHero.SetPersonalRelation(hero, (checkHero.GetRelation(hero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));
                        }
                    }
                }
            }

            //Finally set player as parent
            //May overwrite spouse in homosexual relationships or conflict with mods that change marriage
            if (Hero.MainHero.IsFemale)
            {
                hero.Mother = Hero.MainHero;
            }
            else
            {
                hero.Father = Hero.MainHero;
            }

            Hero.MainHero.Children.Add(hero);

            hero.SetPersonalRelation(Hero.MainHero, (hero.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));

            if (hero.IsPrisoner)
            {
                releaseAdoptedPrisoner(hero);
            }

            changeAdoptedHeroParty(hero);

            OnHeroAdopted(Hero.MainHero, hero, false);

            ACHelper.Print("{=STR_LOG_00005_001}Successfully adopted " + hero.Name + "{=STR_LOG_00005_002} as your new child.");

            return 1;
        }

        //If Hero being adopted is in a warParty we need to update that party to be part of player clan
        private static void changeAdoptedHeroParty(Hero hero)
        {
            try
            {
                if (hero.PartyBelongedTo != null) //in a party
                {
                    if (hero.PartyBelongedTo.ActualClan != null) //party has a clan
                    {
                        WarPartyComponent warPartyComponent;
                        ;
                        if (hero.PartyBelongedTo.PartyComponent != null
                            && (warPartyComponent = (hero.PartyBelongedTo.PartyComponent as WarPartyComponent)) != null) //is a war party
                        {
                            if (Hero.MainHero.Clan != null)
                            {
                                if (hero.PartyBelongedTo.Owner == hero)//person being adopted is leader of the party
                                {
                                    hero.PartyBelongedTo.ActualClan = Hero.MainHero.Clan;
                                }
                                else
                                {
                                    AccessTools.Property(typeof(Hero), "PartyBelongedTo").SetValue(hero, null); //if not leader just remove from party
                                    Hero.MainHero.PartyBelongedTo.AddElementToMemberRoster(hero.CharacterObject, -1);
                                }

                            }
                            else
                            {
                                AccessTools.Property(typeof(Hero), "PartyBelongedTo").SetValue(hero, null);
                                Hero.MainHero.PartyBelongedTo.AddElementToMemberRoster(hero.CharacterObject, -1);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ACHelper.Error("{=STR_LOG_00006}EXCEPTION trying to change adopted hero's party clan!", e);
            }
        }

        //If adopting a prisoner we want to add them to the player's party
        //Note: not using the built in because I want them to go to player's party instead of being released to hometown
        private static void releaseAdoptedPrisoner(Hero hero)
        {
            //EndCaptivityAction.ApplyByRemovedParty(hero);
            try
            {
                hero.ChangeState(Hero.CharacterStates.Active);

                hero.PartyBelongedToAsPrisoner.PrisonRoster.RemoveTroop(hero.CharacterObject);
                AccessTools.Property(typeof(Hero), "PartyBelongedToAsPrisoner").SetValue(hero, null);

                hero.StayingInSettlement = null;

                AccessTools.Method(typeof(Hero), "SetPartyBelongedTo", new Type[] { typeof(MobileParty) }).Invoke(hero, new Object[] { Hero.MainHero.PartyBelongedTo });
                Hero.MainHero.PartyBelongedTo.AddElementToMemberRoster(hero.CharacterObject, 1);
            }
            catch (Exception e)
            {
                ACHelper.Error("{=STR_LOG_00007}EXCEPTION trying to remove adopted hero from prison!", e);
            }
        }


        //This will run logic for is hero is faction ruler or clan leader to chose new leaders
        private static void performHeroClanUpdates(ref Hero hero)
        {
            try
            {
                if (hero.IsFactionLeader || (hero.Clan != null && hero.Clan.Leader == hero))
                {
                    ChangeClanLeaderAction.ApplyWithoutSelectedNewLeader(hero.Clan);
                }
            }
            catch (Exception e)
            {
                ACHelper.Error("{=STR_LOG_00008}EXCEPTION trying to update faction leader clan!", e);
            }

        }

        //It will assign children and spouse to player clan if they need to be
        //it will also check for need to update spouse clan if spouse is a clan leader
        private static void performHeroFamilyUpdates(ref Hero hero)
        {

            //move hero's children and spouse to player clan (siblings and parents will be lost)
            foreach (Hero child in hero.Children)
            {
                if (child.Age < Campaign.Current.Models.AgeModel.HeroComesOfAge)
                {
                    if (child.Occupation != Occupation.Lord)
                    {
                        AccessTools.Property(typeof(Hero), "Occupation").SetValue(child, Occupation.Lord);
                        //ACHelper.Print("Occupation To Lord");
                    }
                    child.IsNoble = true;
                    child.Clan = Hero.MainHero.Clan;
                    child.HasMet = true;
                    child.CompanionOf = null;
                    child.SetPersonalRelation(Hero.MainHero, (child.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));
                    ACHelper.Print("" + child.Name + "{=STR_LOG_00002_001}, child of " + hero.Name + "{=STR_LOG_00002_002}, added to player clan because child has not come of age!");
                }
            }

            //Set spouse to player faction
            if (hero.Spouse != null)
            {
                Hero spouse = hero.Spouse;

                //In case spouse is a clan leader
                performHeroClanUpdates(ref spouse);

                if (spouse.Occupation != Occupation.Lord)
                {
                    AccessTools.Property(typeof(Hero), "Occupation").SetValue(spouse, Occupation.Lord);
                    //ACHelper.Print("Occupation To Lord");
                }

                spouse.IsNoble = true;
                spouse.Clan = Hero.MainHero.Clan;
                spouse.HasMet = true;
                spouse.CompanionOf = null;
                spouse.SetPersonalRelation(Hero.MainHero, (spouse.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));
                ACHelper.Print("" + spouse.Name + "{=STR_LOG_00003_001}, spouse of " + hero.Name + "{=STR_LOG_00003_002}, added to player clan!");
            }
            else
            {
                //above might not work properly for female heros or homosexual relationships so check all heros if they are spouse of hero
                foreach (Hero checkHero in hero.MapFaction.Lords)
                {
                    Hero temp = (Hero)checkHero;
                    if (temp.Spouse != null)
                    {
                        if (temp.Spouse == hero)
                        {
                            performHeroClanUpdates(ref temp);
                            if (temp.Occupation != Occupation.Lord)
                            {
                                AccessTools.Property(typeof(Hero), "Occupation").SetValue(temp, Occupation.Lord);
                                //ACHelper.Print("Occupation To Lord");
                            }
                            temp.IsNoble = true;
                            temp.Clan = Hero.MainHero.Clan;
                            temp.HasMet = true;
                            temp.CompanionOf = null;
                            temp.SetPersonalRelation(Hero.MainHero, (temp.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));
                            ACHelper.Print("" + temp.Name + "{=STR_LOG_00003_001}, spouse of " + hero.Name + "{=STR_LOG_00003_002}, added to player clan!");
                        }
                    }
                }
            }
        }

        //notify player of adoption
        private static void OnHeroAdopted(Hero adopter, Hero adoptedHero, Boolean isSibling)
        {
            TextObject textObject = new("{=STR_NOT_00001}{ADOPTER.LINK} adopted {ADOPTED_HERO.LINK}.", null);
            StringHelpers.SetCharacterProperties("ADOPTER", adopter.CharacterObject, textObject);
            StringHelpers.SetCharacterProperties("ADOPTED_HERO", adoptedHero.CharacterObject, textObject);
            if (isSibling)
            {
                InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/child_born");//what can I use instead??
            }
            else
            {
                InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/child_born");
            }

        }
    }
}

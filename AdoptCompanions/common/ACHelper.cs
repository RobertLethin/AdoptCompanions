using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using AdoptCompanions.Settings;
using MCM.Abstractions.Settings.Base.Global;
using AdoptCompanions.ViewModels;
using TaleWorlds.Localization;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;

namespace AdoptCompanions.Common
{
    internal static class ACHelper
    {
        //This Class uses code from RoGreat and Marry Anyone mod
        public static void Print(string message)
        {
            if (MCM.Abstractions.Settings.Base.Global.GlobalSettings<ACSettings>.Instance.Debug)
            {
                // Custom purple!
                Color color = new(0.6f, 0.2f, 1f);
                InformationManager.DisplayMessage(new InformationMessage(message, color));
            } else
            {
                // Custom purple!
                Color color = new(0.6f, 0.2f, 1f);
                InformationManager.DisplayMessage(new InformationMessage(message, color));
            }
        }

        public static void Error(Exception exception)
        {
            InformationManager.DisplayMessage(new InformationMessage("Adopt Companions: " + exception.Message, Colors.Red));
        }

        //Checks for family relationship between heros.
        //>0 is related. 1 for it is a parent. 2 for it is a sibling. 3 for it is a child. 4 is for spouse
        public static int isFamily(Hero familyHero, Hero checkHero)
        {
            if (familyHero.Father == checkHero
                || familyHero.Mother == checkHero)
            {
                return 1;
            }
            else if (familyHero.Siblings.Contains(checkHero))
            {
                return 2;
            }
            else if (familyHero.Children.Contains(checkHero))
            {
                return 3;
            } else if (familyHero.Spouse == checkHero)
            {
                return 4;
            }

            return 0;
        }
         
        public static int canAdopt(Hero hero)
        {
            int reason = 0;

            //check if already family
            if (ACHelper.isFamily(Hero.MainHero, hero) > 0)
            {
                return AdoptConstants.FAIL_FAMILY;
            }

            //check diplomatic settings
            //Check for different factions
            if (!GlobalSettings<ACSettings>.Instance.canAdoptDifferentFactions && Hero.MainHero.MapFaction != hero.MapFaction)
            {
                return AdoptConstants.FAIL_DIFFERENT_FACTION;
            }

            //check for at war
            if (!GlobalSettings<ACSettings>.Instance.canAdoptAtWar && hero.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
            {
                return AdoptConstants.FAIL_AT_WAR;
            }

            //check for at prisioners
            if (!GlobalSettings<ACSettings>.Instance.canAdoptPrisoners && hero.IsPrisoner)
            {
                return AdoptConstants.FAIL_PRISONER;
            }

            //check for hero type settings
            //Children
            if (hero.IsChild)
            {
                if (GlobalSettings<ACSettings>.Instance.canAdoptChildren)
                {
                    return AdoptConstants.PASS_CHILDREN;
                } else
                {
                    return AdoptConstants.FAIL_CHILDREN;
                }
                
            }

            //Compainions 
            if (GlobalSettings<ACSettings>.Instance.canAdoptCompanions && hero.IsPlayerCompanion)
            {
                //ACHelper.Print("Adpot Companion: passes dialog check");
                return AdoptConstants.PASS_COMPANION;
            }

            //Kings/Queens/faction leaders
            if (!GlobalSettings<ACSettings>.Instance.canAdoptKings && hero.IsFactionLeader)
            {
                return AdoptConstants.FAIL_FACTION_LEADER;
            }
            else if (GlobalSettings<ACSettings>.Instance.canAdoptKings && hero.IsFactionLeader)
            {
                return AdoptConstants.PASS_FACTION_LEADER;
            }

            //Lords
            if (GlobalSettings<ACSettings>.Instance.canAdoptLords && hero.Occupation == Occupation.Lord)
            {
                return AdoptConstants.PASS_LORD;
            }

            //Notables
            if (GlobalSettings<ACSettings>.Instance.canAdoptNotables && hero.IsNotable)
            {
                return AdoptConstants.PASS_NOTABLE;
            }

            //ACHelper.Print("Adpot Companion: fails dialog check");
            return AdoptConstants.FAIL_OTHER;
        }

        public static int checkRelationship(Hero hero)
        {
            //check relationship
            if (GlobalSettings<ACSettings>.Instance.RelationshipMinimum <= hero.GetRelation(Hero.MainHero))
            { 
                return AdoptConstants.PASS_RELATIONSHIP;
            }
            return AdoptConstants.FAIL_RELATIONSHIP;
        }

        public static int AdoptAction(Hero hero, AdoptionTypeVM adoptionType)
        {
            if (canAdopt(hero) < 0 && checkRelationship(hero) == AdoptConstants.FAIL_RELATIONSHIP)
            {
                return -1;
            }
            if (adoptionType.typeId == AdoptConstants.TYPE_ID_SIBLING)
            {
                return AdoptSiblingAction(hero);
            } else if (adoptionType.typeId == AdoptConstants.TYPE_ID_CHILD)
            {
                return AdoptChildAction(hero);
            }

             return -1;
        }

        public static int AdoptSiblingAction(Hero hero)
        {

            //perform logic for updating if clan leader or faction ruler
            //and logic for reassigning family clan if needed (young children and spouse)
            performHeroClanUpdates(ref hero);
            performHeroFamilyUpdates(ref hero);

            //need to be lord to be family
            if (hero.Occupation != Occupation.Lord)
            {
                hero.SetNewOccupation(Occupation.Lord);
                //AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                //ACHelper.Print("Occupation To Lord");
            }

            //remove from being prisoner
            if(hero.IsMercenary)
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

            //AccessTools.Field(typeof(Agent), "_name").SetValue(agent, hero.Name);
            
            if (hero.IsPrisoner)
            {
                releaseAdoptedPrisoner(hero);
            }

           

            OnHeroAdopted(Hero.MainHero, hero, true);

            ACHelper.Print("Successfully adopted " + hero.Name + " as your new sibling.");
            return 0;
        }

        public static int AdoptChildAction(Hero hero)
        {

            //perform logic for updating if clan leader or faction ruler
            //and logic for reassigning family clan if needed (young children and spouse)
            performHeroClanUpdates(ref hero);
            performHeroFamilyUpdates(ref hero);

            if (hero.Occupation != Occupation.Lord)
            {
                hero.SetNewOccupation(Occupation.Lord);
                //AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                //ACHelper.Print("Occupation To Lord");
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

            hero.SetPersonalRelation(Hero.MainHero, (hero.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));

            if (hero.IsPrisoner)
            {
                releaseAdoptedPrisoner(hero);
            }

            changeAdoptedHeroParty(hero);

            //AccessTools.Field(typeof(Agent), "_name").SetValue(agent, hero.Name);
            OnHeroAdopted(Hero.MainHero, hero, false);

            ACHelper.Print("Successfully adopted " + hero.Name + " as your new child.");

            return 1;
        }

        public static void changeAdoptedHeroParty(Hero hero)
        {
            try
            {
                if (hero.PartyBelongedTo != null)
                {
                    if (hero.PartyBelongedTo.ActualClan != null)
                    {
                        WarPartyComponent warPartyComponent;
                        ;
                        if (hero.PartyBelongedTo.PartyComponent != null
                            && (warPartyComponent = (hero.PartyBelongedTo.PartyComponent as WarPartyComponent)) != null)
                        {
                            if (Hero.MainHero.Clan != null)
                            {
                                if(hero.PartyBelongedTo.Owner == hero)
                                {
                                    hero.PartyBelongedTo.ActualClan = Hero.MainHero.Clan;
                                }
                                else
                                {
                                    AccessTools.Property(typeof(Hero), "PartyBelongedTo").SetValue(hero, null);
                                }

                            }
                            else
                            {
                                AccessTools.Property(typeof(Hero), "PartyBelongedTo").SetValue(hero, null);
                            }
                        }
                    }
                }
                /*
                PartyBase partyBase = (hero.PartyBelongedTo != null) ? hero.PartyBelongedTo.Party : hero.CurrentSettlement?.Party;
                hero.CompanionOf = null;
                if (partyBase != null)
                {
                    if (partyBase.LeaderHero != hero)
                    {
                        partyBase.MemberRoster.AddToCounts(hero.CharacterObject, -1);
                    }
                    else
                    {
                        partyBase.MemberRoster.AddToCounts(hero.CharacterObject, -1);
                        partyBase.MobileParty.RemovePartyLeader();
                        if (partyBase.MemberRoster.Count == 0)
                        {
                            DestroyPartyAction.Apply(null, partyBase.MobileParty);
                        }
                        else
                        {
                            DisbandPartyAction.ApplyDisband(partyBase.MobileParty);
                        }
                    }
                }
                */
            } catch (Exception e) {
                ACHelper.Print("ADOPT COMPANIONS: EXCEPTION trying to change adopted hero's party clan!");
            }
        }

        public static void releaseAdoptedPrisoner(Hero hero)
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
            } catch (Exception e)
            {
                ACHelper.Print("ADOPT COMPANIONS: EXCEPTION trying to remove adopted hero from prison!");
            }
        }


        //This will run logic for is hero is faction ruler or clan leader to chose new leaders
        private static void performHeroClanUpdates(ref Hero hero)
        {
            try
            {
                if (hero.IsFactionLeader || (hero.Clan != null && hero.Clan.Leader == hero))
                {
                    TaleWorlds.CampaignSystem.Actions.ChangeClanLeaderAction.ApplyWithoutSelectedNewLeader(hero.Clan);
                }
                
                /*
                //if they are faction leader need to assign a new leader
                if (hero.IsFactionLeader)
                {
                    //chose new ruling clan
                    Clan newRulingClan = new Clan();
                    bool foundReplacementClan = false;
                    foreach (Clan factionClan in hero.Clan.Kingdom.Clans)
                    {
                        if (hero.Clan != factionClan && factionClan.TotalStrength > newRulingClan.TotalStrength)
                        {
                            newRulingClan = factionClan;
                            foundReplacementClan = true;
                        }
                    }

                    //if new ruling clan replacement found
                    if (foundReplacementClan)
                    {
                        hero.Clan.Kingdom.RulingClan = newRulingClan;
                    }
                    //no clan to replace hero's clan. Instead use hero's clan
                    else
                    {  
                        findNewClanLeader(hero);
                    }
                } else
                {
                    findNewClanLeader(hero);
                }
                */

            } catch (Exception e)
            {
                ACHelper.Print("ADOPT COMPANIONS: EXCEPTION trying to update faction leader clan!");
            }

        }

        public static void findNewClanLeader(Hero hero)
        {
            //Chose new leader for hero clan
            if (hero.Clan != null)
            {
                if (hero.Clan.Leader == hero)
                {
                    //Can chose new leader
                    MBReadOnlyList<Hero> clanMembers = hero.Clan.Lords;

                    int livingMembers = 0;
                    foreach (Hero member in clanMembers)
                    {
                        if (member.IsAlive)
                        {
                            livingMembers++;
                        }
                    }

                    if (livingMembers > 1)
                    {
                        Hero newLeader = hero.Clan.Lords.First();

                        foreach (Hero clanHero in hero.Clan.Lords)
                        {
                            if (hero.GetRelation(clanHero) > hero.GetRelation(newLeader))
                            {
                                newLeader = clanHero;
                            }
                        }
                        hero.Clan.SetLeader(newLeader);
                    }
                    else
                    {
                        ACHelper.Print("Hero only living member in clan! Clan is eliminated!");
                        //hero.Clan.IsEliminated = true; 
                        //hero.Clan.SetLeader(null);
                        Clan temp = hero.Clan;
                        hero.Clan = Hero.MainHero.Clan;
                        //AccessTools.Property(typeof(Clan), "_isEliminated").SetValue(hero.Clan, true);
                        //AccessTools.Property(typeof(Clan), "IsEliminated").SetValue(temp, true);
                        AccessTools.Method(typeof(Clan), "DeactivateClan").Invoke(temp, null);
                        //hero.Clan = Hero.MainHero.Clan;
                    }

                }
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
                    ACHelper.Print("" + child.Name + ", child of " + hero.Name + ", added to player clan because child has not come of age!");
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
                ACHelper.Print("" + spouse.Name + ", spouse of " + hero.Name + ", added to player clan!");
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
                            ACHelper.Print("" + temp.Name + ", spouse of " + hero.Name + ", added to player clan!");
                        }
                    }
                }
            }
        }

        private static void OnHeroAdopted(Hero adopter, Hero adoptedHero, Boolean isSibling)
        {
            TextObject textObject = new("{=adopted}{ADOPTER.LINK} adopted {ADOPTED_HERO.LINK}.", null);
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

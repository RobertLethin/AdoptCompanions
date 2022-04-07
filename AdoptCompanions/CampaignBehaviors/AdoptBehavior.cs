using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using SandBox;

using HarmonyLib;
using Helpers;
using AdoptCompanions.Settings;
using MCM.Abstractions.Settings.Base.Global;

namespace AdoptCompanions.CampaignBehaviors
{
    internal class AdoptBehavior : CampaignBehaviorBase
    {
        protected void AddDialogs(CampaignGameStarter starter)
        {
            //Companions
            starter.AddPlayerLine("adoption_discussion_AC", "hero_main_options", "adoption_companion_start_AC", "I feel a strong bond with you and I would be honored to have you join my family.", new ConversationSentence.OnConditionDelegate(conversation_adopt_on_condition), null, 70, null, null);
            starter.AddDialogLine("character_adoption_start_res_p_AC", "adoption_companion_start_AC", "adoption_companion_choice_AC", "I have always thought of you as my chosen family and would love to be a part of your actaul family. Were you planning on adopting me as your child or sibling? [rf:happy][rb:very_positive]", new ConversationSentence.OnConditionDelegate(conversation_relationship_pass_condition), null, 100, null);
            starter.AddDialogLine("character_adoption_start_res_f_AC", "adoption_companion_start_AC", "hero_main_options", "I'm sorry, but I don't like you enough to agree to that. [rf:angry][rb:negative]", new ConversationSentence.OnConditionDelegate(conversation_relationship_fail_condition_1), null, 100, null);

            starter.AddPlayerLine("adoption_discussion_sibling_AC", "adoption_companion_choice_AC", "adoption_companion_res_sibling_AC", "I see you as my equal and want you to be my {?CONVERSATION_CHARACTER.GENDER}sister{?}brother{\\?}!", null, null, 120, null, null);
            starter.AddDialogLine("character_adoption_response_sibling_AC", "adoption_companion_res_sibling_AC", "hero_main_options", "Nothing would make me happier, {?PLAYER.GENDER}sister{?}brother{\\?}! [rf:happy][rb:very_positive]", null, new ConversationSentence.OnConsequenceDelegate(conversation_adopt_sibling_on_consequence), 100, null);

            starter.AddPlayerLine("adoption_discussion_child_AC", "adoption_companion_choice_AC", "adoption_companion_res_child_AC", "I see you as my protege and want you to be my {?CONVERSATION_CHARACTER.GENDER}daughter{?}son{\\?}!", null, null, 100, null, null);
            starter.AddDialogLine("character_adoption_response_child_AC", "adoption_companion_res_child_AC", "hero_main_options", "Nothing would make me happier, {?PLAYER.GENDER}Mother{?}Father{\\?}! [rf:happy][rb:very_positive]", null, new ConversationSentence.OnConsequenceDelegate(conversation_adopt_child_on_consequence), 100, null);

            starter.AddPlayerLine("adoption_discussion_cancel_AC", "adoption_companion_choice_AC", "hero_main_options", "Sorry, I still need to think about it more.", null, null, 50, null, null);
        }


        private bool conversation_adopt_on_condition()
        {
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);

            Hero hero = Hero.OneToOneConversationHero;

            //check if already family
            if (ACHelper.isFamily(Hero.MainHero, hero) > 0)
            {
                //ACHelper.Print("Already fam");
                return false;
            }

            //check diplomatic settings
            //Check for different factions
            if (!GlobalSettings<ACSettings>.Instance.canAdoptDifferentFactions && Hero.MainHero.MapFaction != hero.MapFaction)
            {
                return false;
            }

            //check for at war
            if (!GlobalSettings<ACSettings>.Instance.canAdoptAtWar && hero.MapFaction.IsAtWarWith(Hero.MainHero.MapFaction))
            {
                return false;
            }

            //check for at prisioners
            if (!GlobalSettings<ACSettings>.Instance.canAdoptPrisoners && hero.IsPrisoner)
            {
                return false;
            }

            //check for hero type settings
            //Compainions 
            if (GlobalSettings<ACSettings>.Instance.canAdoptCompanions && hero.IsPlayerCompanion)
            {
                //ACHelper.Print("Adpot Companion: passes dialog check");
                return true;
            }
            //Kings/Queens/faction leaders
            if (!GlobalSettings<ACSettings>.Instance.canAdoptKings && hero.IsFactionLeader)
            {
                return false;
            }
            else if (GlobalSettings<ACSettings>.Instance.canAdoptKings && hero.IsFactionLeader)
            {
                return true;
            }

            //Lords
            if (GlobalSettings<ACSettings>.Instance.canAdoptLords && hero.Occupation == Occupation.Lord)
            {
                return true;
            }

            //Notables
            if (GlobalSettings<ACSettings>.Instance.canAdoptNotables && hero.IsNotable)
            {
                return true;
            }

            //ACHelper.Print("Adpot Companion: fails dialog check");
            return false;

        }

        private int adoptionFail = 0;

        private bool conversation_relationship_pass_condition()
        {
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);

            Hero hero = Hero.OneToOneConversationHero;

            //check relationship
            if (GlobalSettings<ACSettings>.Instance.RelationshipMinimum <= hero.GetRelation(Hero.MainHero))
            {
                adoptionFail = 0;
                return true;
            }


            adoptionFail = 1;
            return false;
        }

        //failed due to relationship
        private bool conversation_relationship_fail_condition_1()
        {
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);

            Hero hero = Hero.OneToOneConversationHero;

            if (adoptionFail == 1)
            {
                hero.SetPersonalRelation(Hero.MainHero, (hero.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainFail));
                return true;
            }

            return false;
        }

        private void conversation_adopt_sibling_on_consequence()
        {

            //Agent agent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;
            Hero hero = Hero.OneToOneConversationHero;

            //perform logic for updating if clan leader or faction ruler
            //and logic for reassigning family clan if needed (young children and spouse)
            performHeroClanUpdates(ref hero);
            performHeroFamilyUpdates(ref hero);


            if (hero.Occupation != Occupation.Lord)
            {
                AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                //ACHelper.Print("Occupation To Lord");
            }

            hero.IsNoble = true;
            hero.Clan = Hero.MainHero.Clan;
            hero.HasMet = true;
            hero.CompanionOf = null;
            //set same parents as player (which effectively makes them siblings)
            hero.Father = Hero.MainHero.Father;
            hero.Mother = Hero.MainHero.Mother;

            //Increase relationship by 30 (old version hard set it to 100)
            hero.SetPersonalRelation(Hero.MainHero, (hero.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainPass));

            //AccessTools.Field(typeof(Agent), "_name").SetValue(agent, hero.Name);

            OnHeroAdopted(Hero.MainHero, hero, true);

            ACHelper.Print("Successfully adopted " + hero.Name + " as your new sibling.");
        }

        private void conversation_adopt_child_on_consequence()
        {
            //Agent agent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;

            Hero hero = Hero.OneToOneConversationHero;

            //perform logic for updating if clan leader or faction ruler
            //and logic for reassigning family clan if needed (young children and spouse)
            performHeroClanUpdates(ref hero);
            performHeroFamilyUpdates(ref hero);

            if (hero.Occupation != Occupation.Lord)
            {
                AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                //ACHelper.Print("Occupation To Lord");
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


            //AccessTools.Field(typeof(Agent), "_name").SetValue(agent, hero.Name);
            OnHeroAdopted(Hero.MainHero, hero, false);

            ACHelper.Print("Successfully adopted " + hero.Name + " as your new child.");
        }

        //This will run logic for is hero is faction ruler or clan leader to chose new leaders
        private void performHeroClanUpdates(ref Hero hero)
        {
            //if they are faction leader need to assign a new leader
            if (hero.IsFactionLeader)
            {
                //chose new ruling clan
                Clan newRulingClan = new Clan();

                foreach (Clan factionClan in hero.Clan.Kingdom.Clans)
                {
                    if (hero.Clan != factionClan && factionClan.TotalStrength > newRulingClan.TotalStrength)
                    {
                        newRulingClan = factionClan;
                    }
                }
                hero.Clan.Kingdom.RulingClan = newRulingClan;
            }

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
        private void performHeroFamilyUpdates(ref Hero hero)
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

        private void OnHeroAdopted(Hero adopter, Hero adoptedHero, Boolean isSibling)
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

        public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            /*            foreach(Hero hero in Hero.AllAliveHeroes.ToList())
                        {
                            if (Hero.MainHero.Siblings.Contains(hero))
                            {
                                if (hero.Occupation != Occupation.Lord)
                                {
                                    AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                                    //ACHelper.Print("Occupation To Lord");
                                }

                            } else if (Hero.MainHero.Children.Contains(hero))
                            {
                                if (hero.Occupation != Occupation.Lord)
                                {
                                    AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                                    //ACHelper.Print("Occupation To Lord");
                                }
                            }
                        }
            */

            AddDialogs(campaignGameStarter);
        }

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(OnSessionLaunched));
        }
        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;


using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using SandBox;


using Helpers;
using AdoptCompanions.Settings;
using TaleWorlds.CampaignSystem.Conversation;

namespace AdoptCompanions.CampaignBehaviors
{
    internal class AdoptBehavior : CampaignBehaviorBase
    {
        protected void AddDialogs(CampaignGameStarter starter)
        {
            //Companions
            starter.AddPlayerLine("adoption_discussion_AC", "hero_main_options", "adoption_companion_start_AC", "I feel a strong bond with you and I would be honored to have you join my family.", new ConversationSentence.OnConditionDelegate(conversation_adopt_on_condition), null, 70, null, null);
            starter.AddDialogLine("character_adoption_start_res_AC", "adoption_companion_start_AC", "adoption_companion_choice_AC", "I have always thought of you as my chosen family and would love to be a part of your actaul family. Were you planning on adopting me as your child or sibling? [rf:happy][rb:very_positive]", null, null, 100, null);
            
            starter.AddPlayerLine("adoption_discussion_sibling_AC", "adoption_companion_choice_AC", "adoption_companion_res_sibling_AC", "I see you as my equal and want you to be my {?CONVERSATION_CHARACTER.GENDER}sister{?}brother{\\?}!", null, null, 120, null, null);
            starter.AddDialogLine("character_adoption_response_sibling_AC", "adoption_companion_res_sibling_AC", "hero_main_options", "Nothing would make me happier, {?PLAYER.GENDER}sister{?}brother{\\?}! [rf:happy][rb:very_positive]", null, new ConversationSentence.OnConsequenceDelegate(conversation_adopt_sibling_on_consequence), 100, null);

            starter.AddPlayerLine("adoption_discussion_child_AC", "adoption_companion_choice_AC", "adoption_companion_res_child_AC", "I see you as my protege and want you to be my {?CONVERSATION_CHARACTER.GENDER}daughter{?}son{\\?}!", null, null, 100, null, null);
            starter.AddDialogLine("character_adoption_response_child_AC", "adoption_companion_res_child_AC", "hero_main_options", "Nothing would make me happier, {?PLAYER.GENDER}Mother{?}Father{\\?}! [rf:happy][rb:very_positive]", null, new ConversationSentence.OnConsequenceDelegate(conversation_adopt_child_on_consequence), 100, null);

            starter.AddPlayerLine("adoption_discussion_cancel_AC", "adoption_companion_choice_AC", "hero_main_options", "Sorry, I still need to think about it more.", null, null, 50, null, null);
        }


        private bool conversation_adopt_on_condition()
        {
            ISettingsProvider settings = new ACSettings();
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);

            Hero hero = Hero.OneToOneConversationHero;

            if (hero.IsPlayerCompanion)
            {
                //ACHelper.Print("Adpot Companion: passes dialog check");
                return true;
            }

            //ACHelper.Print("Adpot Companion: fails dialog check");
            return false;

        }

        private void conversation_adopt_sibling_on_consequence()
        {

            //Agent agent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;
            Hero hero = Hero.OneToOneConversationHero;

            if (hero.Occupation != Occupation.Lord)
            {
                ACHelper.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                //AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                //ACHelper.Print("Occupation To Lord");
            }

            hero.IsNoble = true;
            hero.Clan = Hero.MainHero.Clan;
            hero.HasMet = true;
            hero.CompanionOf = null;
            //set same parents as player (which effectively makes them siblings)
            hero.Father =  Hero.MainHero.Father;
            hero.Mother = Hero.MainHero.Mother;

            //Increase relationship by 30 (old version hard set it to 100)
            hero.SetPersonalRelation(Hero.MainHero, (Hero.MainHero.GetRelation(hero) + 30));

            //AccessTools.Field(typeof(Agent), "_name").SetValue(agent, hero.Name);

            OnHeroAdopted(Hero.MainHero, hero, true);

            ACHelper.Print("Successfully adopted " + hero.Name + " as your new sibling.");
        }

        private void conversation_adopt_child_on_consequence()
        {
            //Agent agent = (Agent)Campaign.Current.ConversationManager.OneToOneConversationAgent;

            Hero hero = Hero.OneToOneConversationHero;

            if (hero.Occupation != Occupation.Lord)
            {
                ACHelper.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                //AccessTools.Property(typeof(Hero), "Occupation").SetValue(hero, Occupation.Lord);
                //ACHelper.Print("Occupation To Lord");
            }

            hero.IsNoble = true;
            hero.Clan = Hero.MainHero.Clan;
            hero.HasMet = true;
            hero.CompanionOf = null;
            //First set spouse as parent
            //Should be first in case homosexual marriage causes issues to ensure player always gets set as parent at minimum
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

                hero.SetPersonalRelation(Hero.MainHero.Spouse, (Hero.MainHero.Spouse.GetRelation(hero) + 30));
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

            hero.SetPersonalRelation(Hero.MainHero, (Hero.MainHero.GetRelation(hero) + 30));
            

            //AccessTools.Field(typeof(Agent), "_name").SetValue(agent, hero.Name);
            OnHeroAdopted(Hero.MainHero, hero, false);

            ACHelper.Print("Successfully adopted " + hero.Name + " as your new child.");
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

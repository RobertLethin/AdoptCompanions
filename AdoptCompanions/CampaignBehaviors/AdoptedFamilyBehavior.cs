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

using Helpers;
using AdoptCompanions.Settings;
using TaleWorlds.CampaignSystem.Conversation;

namespace AdoptCompanions.CampaignBehaviors
{
    internal class AdoptedFamilyBehavior : CampaignBehaviorBase
    {
        protected void AddDialogs(CampaignGameStarter starter)
        {
            //Companions
            starter.AddPlayerLine("rename_discussion_AC", "hero_main_options", "rename_family_response_AC", "I think you should be known be a new name.", new ConversationSentence.OnConditionDelegate(conversation_rename_on_condition), null, 70, null, null);
            starter.AddDialogLine("character_rename_res_AC", "rename_family_response_AC", "rename_family_player_res_AC", "I was just thinking of changing my name... What name do you think I should chose?", null, new ConversationSentence.OnConsequenceDelegate(conversation_rename_on_consequence), 100, null);

            //starter.AddPlayerLine("rename_discussion_AC", "rename_family_player_res_AC", "rename_family_confirm_AC", "You should name yourself...", null, null, 100, null, null);
            //starter.AddDialogLine("character_change_name_confirm_AC", "rename_family_confirm_AC", "hero_main_options", "I love that name, from now on I shall use it!", null, null, 100, null);
            //starter.AddPlayerLine("rename_discussion_AC", "rename_family_player_res_AC", "rename_family_confirm_AC", "You should name yourself...", null, null, 100, null, null);
            starter.AddDialogLine("character_change_name_confirm_AC", "rename_family_player_res_AC", "hero_main_options", "I love that name, from now on I shall use it!", null, null, 100, null);

            //starter.AddPlayerLine("rename_discussion_confirm_AC", "rename_family_player_confirm_AC", "hero_main_options", "I'm glad you like it. Everyone shall call you {CONVERSATION_CHARACTER.NAME} now!", null, null, 100, null, null);

        }

        private bool conversation_rename_on_condition()
        {
            ISettingsProvider settings = new ACSettings();
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);
            
            Hero hero = Hero.OneToOneConversationHero;

            if (ACHelper.isFamily(Hero.MainHero, hero) > 0)
            {
                //ACHelper.Print("Adpot Companion: passes dialog check");
                return true;

            } else if (hero.IsPlayerCompanion)
            {
                return false; //disabled to work with distiguished service but could change in future to allow this mod to do this too.
            }

            //ACHelper.Print("Adpot Companion: fails dialog check");
            return false;
        }
        private void conversation_rename_on_consequence()
        {
            ISettingsProvider settings = new ACSettings();
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);

            Hero hero = Hero.OneToOneConversationHero;
            InformationManager.ShowTextInquiry(new TextInquiryData(
                "Enter New Name: ", 
                "", 
                true, 
                false, 
                ((object)GameTexts.FindText("str_done", null)).ToString(), 
                null, 
                new Action<string>(change_hero_name), 
                null, 
                false, 
                null, 
                "", hero.FirstName.ToString()), false);
        }

        private void change_hero_name(string s)
        {
            Hero hero = Hero.OneToOneConversationHero;
            hero.SetName(new TextObject(s, null), new TextObject(s, null));

            ACHelper.Print("Adpot Companion: Changed family member's name to " + s);
        }

        //namechanceconsequence() => InformationManager.ShowTextInquiry(new TextInquiryData("Create a new name: ", string.Empty, true, false, ((object)GameTexts.FindText("str_done", (string)null)).ToString(), (string)null, new Action<string>(this.change_hero_name), (Action)null, false, (Func<string, Tuple<bool, string>>)null, "", ""), false);

        public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
            /*
            foreach(Hero hero in Hero.AllAliveHeroes.ToList())
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

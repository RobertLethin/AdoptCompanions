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
using AdoptCompanions.common;
using AdoptCompanions.ViewModels;
using AdoptCompanions.Actions;
using TaleWorlds.CampaignSystem.Conversation;

namespace AdoptCompanions.CampaignBehaviors
{
    internal class AdoptBehavior : CampaignBehaviorBase
    {
        protected void AddDialogs(CampaignGameStarter starter)
        {
            //Adopting
            //lord start (in I have something to discuss)
            starter.AddPlayerLine("adoption_discussion_AC", "lord_talk_speak_diplomacy_2", "adoption_companion_start_AC", "{=STR_CONV_00001}I feel a strong bond with you and I would be honored to have you join my family.", new ConversationSentence.OnConditionDelegate(conversation_adopt_on_condition_Lord), null, 70, null, null);
            //companion start (in About your position in the clan)
            starter.AddPlayerLine("adoption_discussion_AC", "companion_role", "adoption_companion_start_AC", "{=STR_CONV_00001}I feel a strong bond with you and I would be honored to have you join my family.", new ConversationSentence.OnConditionDelegate(conversation_adopt_on_condition_companion), null, 105, null, null);
            //notable start (in I have a quick question)
            starter.AddPlayerLine("adoption_discussion_AC", "lord_talk_ask_something_2", "adoption_companion_start_AC", "{=STR_CONV_00001}I feel a strong bond with you and I would be honored to have you join my family.", new ConversationSentence.OnConditionDelegate(conversation_adopt_on_condition_notable), null, 120, null, null);
            
            starter.AddDialogLine("character_adoption_start_res_p_AC", "adoption_companion_start_AC", "adoption_companion_choice_AC", "{=STR_CONV_00002}I have always thought of you as my chosen family and would love to be a part of your actaul family. Were you planning on adopting me as your child or sibling? [rf:happy][rb:very_positive]", new ConversationSentence.OnConditionDelegate(conversation_relationship_pass_condition), null, 100, null);
            starter.AddDialogLine("character_adoption_start_res_f_AC", "adoption_companion_start_AC", "hero_main_options", "{=STR_CONV_00003}I'm sorry, but I don't like you enough to agree to that. [rf:angry][rb:negative]", new ConversationSentence.OnConditionDelegate(conversation_relationship_fail_condition_1), null, 100, null);

            starter.AddPlayerLine("adoption_discussion_sibling_AC", "adoption_companion_choice_AC", "adoption_companion_res_sibling_AC", "{=STR_CONV_00004}I see you as my equal and want you to be my {?CONVERSATION_CHARACTER.GENDER}sister{?}brother{\\?}!", null, null, 120, null, null);
            starter.AddDialogLine("character_adoption_response_sibling_AC", "adoption_companion_res_sibling_AC", "hero_main_options", "{=STR_CONV_00005}Nothing would make me happier, {?PLAYER.GENDER}sister{?}brother{\\?}! [rf:happy][rb:very_positive]", null, new ConversationSentence.OnConsequenceDelegate(conversation_adopt_sibling_on_consequence), 100, null);

            starter.AddPlayerLine("adoption_discussion_child_AC", "adoption_companion_choice_AC", "adoption_companion_res_child_AC", "{=STR_CONV_00006}I see you as my protege and want you to be my {?CONVERSATION_CHARACTER.GENDER}daughter{?}son{\\?}!", null, null, 100, null, null);
            starter.AddDialogLine("character_adoption_response_child_AC", "adoption_companion_res_child_AC", "hero_main_options", "{=STR_CONV_00007}Nothing would make me happier, {?PLAYER.GENDER}Mother{?}Father{\\?}! [rf:happy][rb:very_positive]", null, new ConversationSentence.OnConsequenceDelegate(conversation_adopt_child_on_consequence), 100, null);

            starter.AddPlayerLine("adoption_discussion_cancel_AC", "adoption_companion_choice_AC", "hero_main_options", "{=STR_CONV_00008}Sorry, I still need to think about it more.", null, null, 50, null, null);

            //Unadopting
            //starter.AddPlayerLine("adoption_discussion_AC", "hero_main_options", "adoption_companion_start_AC", "I feel a strong bond with you and I would be honored to have you join my family.", new ConversationSentence.OnConditionDelegate(conversation_adopt_on_condition), null, 70, null, null);

        }


        private bool conversation_adopt_on_condition_Lord()
        {
            Hero hero = Hero.OneToOneConversationHero;

            return (conversation_adopt_on_condition() && hero.IsNoble);
        }

        private bool conversation_adopt_on_condition_companion()
        {
            Hero hero = Hero.OneToOneConversationHero;

            return (conversation_adopt_on_condition() && hero.IsPlayerCompanion);
        }

        private bool conversation_adopt_on_condition_notable()
        {
            Hero hero = Hero.OneToOneConversationHero;

            return (conversation_adopt_on_condition() && hero.IsNotable);
        }



        private bool conversation_adopt_on_condition()
        {
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);

            Hero hero = Hero.OneToOneConversationHero;

            int canAdopt = ACHelper.canAdopt(hero);

            if (canAdopt > 0 ) return true;
            else return false;
        }

        private int _relCheck = 0;

        private bool conversation_relationship_pass_condition()
        {
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);

            Hero hero = Hero.OneToOneConversationHero;
            _relCheck = ACHelper.checkRelationship(hero);

            if (_relCheck == AdoptConstants.PASS_RELATIONSHIP) return true;
            else return false;
        }

        //failed due to relationship
        private bool conversation_relationship_fail_condition_1()
        {
            StringHelpers.SetCharacterProperties("CONVERSATION_CHARACTER", CharacterObject.OneToOneConversationCharacter);

            Hero hero = Hero.OneToOneConversationHero;

            if (_relCheck == AdoptConstants.FAIL_RELATIONSHIP)
            {
                hero.SetPersonalRelation(Hero.MainHero, (hero.GetRelation(Hero.MainHero) + GlobalSettings<ACSettings>.Instance.RelationshipGainFail));
                return true;
            }

            return false;
        }

        private void conversation_adopt_sibling_on_consequence()
        {
            Hero hero = Hero.OneToOneConversationHero;

            AdoptionTypeVM adoptionType = new AdoptionTypeVM(AdoptConstants.TYPE_ID_SIBLING);
            AdoptActions.AdoptAction(hero, adoptionType);
        }

        private void conversation_adopt_child_on_consequence()
        {
            Hero hero = Hero.OneToOneConversationHero;

            AdoptionTypeVM adoptionType = new AdoptionTypeVM(AdoptConstants.TYPE_ID_CHILD);
            AdoptActions.AdoptAction(hero, adoptionType);
        }

        public void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {
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
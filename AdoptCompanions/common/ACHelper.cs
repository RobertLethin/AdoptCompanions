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
using AdoptCompanions.common;
using MCM.Abstractions.Settings.Base.Global;
using AdoptCompanions.ViewModels;
using TaleWorlds.Localization;
using Helpers;
using TaleWorlds.CampaignSystem.Actions;

namespace AdoptCompanions.common
{
    internal static class ACHelper
    {
        public static void Print(string message)
        {
            /*
            if (MCM.Abstractions.Settings.Base.Global.GlobalSettings<ACSettings>.Instance.Debug)
            {
                // Custom purple!
                Color color = new(0.6f, 0.2f, 1f);
                InformationManager.DisplayMessage(new InformationMessage("Adopt Companions: " + message, color));
            } else
            {
                // Custom purple!
                Color color = new(0.6f, 0.2f, 1f);
                InformationManager.DisplayMessage(new InformationMessage("Adopt Companions: " + message, color));
            }
            */
            // Custom purple!
            Color color = new(0.6f, 0.2f, 1f);
            TextObject msgTextObject = new TextObject(message);
            InformationManager.DisplayMessage(new InformationMessage("Adopt Companions: " + msgTextObject, color));
        }

        public static void Error(String message, Exception exception)
        {
            TextObject msgTextObject = new TextObject(message);
            InformationManager.DisplayMessage(new InformationMessage("Adopt Companions: " + message, Colors.Red));
            InformationManager.DisplayMessage(new InformationMessage("Adopt Companions: " + exception.Message, Colors.Red));
        }

        //Checks for family relationship between heros.
        //>0 is related. 1 for it is a parent. 2 for it is a sibling. 3 for it is a child. 4 is for spouse
        public static int isFamily(Hero familyHero, Hero checkHero)
        {
            if (familyHero.Father == checkHero
                || familyHero.Mother == checkHero)
            {
                return AdoptConstants.FAMILY_PARENT;
            }
            else if (familyHero.Siblings.Contains(checkHero))
            {
                return AdoptConstants.FAMILY_SIBLING;
            }
            else if (familyHero.Children.Contains(checkHero))
            {
                return AdoptConstants.FAMILY_CHILD;
            } 
            else if (familyHero.Spouse == checkHero)
            {
                return AdoptConstants.FAMILY_SPOUSE;
            }

            return AdoptConstants.NOT_FAMILY;
        }
         
        public static int canAdopt(Hero hero)
        {
            int reason = 0;

            if(hero.IsSpecial)//quest people?
            {
                return AdoptConstants.FAIL_OTHER;
            }
            //Cant adopt self
            if (hero == Hero.MainHero)
            {
                return AdoptConstants.FAIL_OTHER;
            }

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

    }
}

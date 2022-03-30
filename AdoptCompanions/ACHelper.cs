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

namespace AdoptCompanions
{
    internal static class ACHelper
    {
        //This Class uses code from RoGreat and Marry Anyone mod
        public static void Print(string message)
        {
            ISettingsProvider settings = new ACSettings();
            if (settings.Debug)
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
        //>0 is related. 1 for it is a parent. 2 for it is a sibling. 3 for it is a child.
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
            }

            return 0;
        }
    }
}

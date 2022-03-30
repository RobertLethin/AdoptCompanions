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

        public static void OccupationToLord(CharacterObject character)
        {
            if (character.Occupation != Occupation.Lord)
            {
                AccessTools.Property(typeof(CharacterObject), "Occupation").SetValue(character, Occupation.Lord);
                Print("Occupation To Lord");
                AccessTools.Field(typeof(CharacterObject), "_originCharacter").SetValue(character, CharacterObject.PlayerCharacter);
                AccessTools.Field(typeof(CharacterObject), "_originCharacterStringId").SetValue(character, CharacterObject.PlayerCharacter.StringId);
            }
        }
    }
}

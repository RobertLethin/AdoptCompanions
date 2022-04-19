using AdoptCompanions.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using static TaleWorlds.CampaignSystem.Actions.KillCharacterAction;

namespace AdoptCompanions.Common
{
    internal class ASHelper
    {
        public static void Print(string message)
        {
            if (MCM.Abstractions.Settings.Base.Global.GlobalSettings<ASSettings>.Instance.Debug)
            {
                // Custom purple!
                Color color = new(0.6f, 0.2f, 1f);
                InformationManager.DisplayMessage(new InformationMessage(message, color));
            }
            else
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
        public static int CanAssassinate(Hero hero)
        {
            if (!hero.CanDie(KillCharacterActionDetail.Murdered))
            {
                Print("HERO CAN'T BE MURDERED");
                return ASConstants.FAIL_CANT_DIE_MURDER;
            }

            if (!hero.IsAlive)
                return ASConstants.FAIL_IS_DEAD;

            return 1;
        }

        public static int AssassinSent(Hero hero)
        {

            return 0;
        }
    }
}

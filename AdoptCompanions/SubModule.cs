using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using AdoptCompanions.CampaignBehaviors;
using AdoptCompanions.Settings;
using Bannerlord.UIExtenderEx;
using HarmonyLib;


namespace AdoptCompanions
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly string HarmonyDomain = "mod.bannerlord.adoptcompanions";

        protected override void OnSubModuleLoad()
        {
            //PatchManager.PatchAll(SubModule.HarmonyDomain);
            //new Harmony(SubModule.HarmonyDomain).PatchAll();
            new Harmony(((object)this).GetType().Namespace).PatchAll(((object)this).GetType().Assembly);
            base.OnSubModuleLoad();
            

        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            if( game.GameType is Campaign)
            {

                CampaignGameStarter gameStarter = (CampaignGameStarter)gameStarterObject;

                gameStarter.AddBehavior(new AdoptBehavior());
                gameStarter.AddBehavior(new AdoptedFamilyBehavior());
            }
        }
    }
}
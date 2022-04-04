﻿using TaleWorlds.CampaignSystem;
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
        protected override void OnSubModuleLoad()
        {
            new Harmony("mod.bannerlord.adoptcompanions").PatchAll();
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
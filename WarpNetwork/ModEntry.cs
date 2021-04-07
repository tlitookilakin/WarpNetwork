using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using SpaceCore.Events;
using System.Collections.Generic;
using System.IO;

namespace WarpNetwork
{
    class ModEntry : Mod, IAssetLoader
    {
        //const
        public static readonly string pathLocData = Path.Combine("Data","WarpNetwork","Destinations");
        public static readonly string pathItemData = Path.Combine("Data","WarpNetwork","WarpItems");

        //main
        internal static Config Config;
        public static API api = new API();
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<Config>();
            CommandHandler.Init(Monitor, helper);
            WarpHandler.Init(Monitor, helper, Config);
            ItemHandler.Init(Monitor, helper, Config);
            Utils.Init(helper, Monitor);
            ObeliskPatch.Init(Monitor, Config);
            DataPatcher.Init(Monitor, helper, Config);
            CPIntegration.Init(Monitor, helper, Config);
            helper.ConsoleCommands.Add("warpnet", "Master command for Warp Network mod. Use 'warpnet' or 'warpnet help' to see a list of subcommands.", CommandHandler.Main);
            helper.Content.AssetEditors.Add(new DataPatcher());
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.Input.ButtonPressed += ItemHandler.ButtonPressed;
            helper.Events.Player.Warped += ObeliskPatch.MoveAfterWarp;
            SpaceEvents.ActionActivated += WarpHandler.HandleAction;
        }
        public void GameLaunched(object sender, GameLaunchedEventArgs ev)
        {
            Config.RegisterModConfigMenu(Helper, ModManifest);
            CPIntegration.AddTokens(ModManifest);
        }
        public override object GetApi()
        {
            return api;
        }
        //loading
        public bool CanLoad<T>(IAssetInfo asset)
        {
            return (
                asset.AssetNameEquals(pathLocData) ||
                asset.AssetNameEquals(pathItemData)
            );
        }
        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals(pathItemData))
            {
                Dictionary<string, WarpItem> items = Helper.Content.Load<Dictionary<string, WarpItem>>(Path.Combine("assets", "WarpItems.json"));
                DataPatcher.AddApiItems(items);
                return (T)(object)items;
            } else if (asset.AssetNameEquals(pathLocData))
            {
                Dictionary<string, WarpLocation> locs = Helper.Content.Load<Dictionary<string, WarpLocation>>(Path.Combine("assets", "Destinations.json"));
                DataPatcher.EditLocationsEnabled(locs);
                DataPatcher.AddApiLocs(locs);
                return (T)(object)locs;
            }
            return default;
        }
    }
}

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WarpNetwork.api;
using WarpNetwork.framework;
using WarpNetwork.models;

namespace WarpNetwork
{
	class ModEntry : Mod
	{
		internal static string AssetPath;
		internal static string LegacyAssetPath = "Data/WarpNetwork";

		//const
		public static readonly string pathLocData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Destinations");
		public static readonly string pathItemData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/WarpItems");
		public static readonly string pathIcons = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Icons");
		public static readonly string pathObjectData = PathUtilities.NormalizeAssetName("Data/WarpNetwork/Objects");
		internal static readonly HashSet<string> knownIcons = new(new[] {"DEFAULT", "farm", "mountain", "island", "desert", "beach", "RETURN"});

		//main
		internal static Config config;
		internal static IModHelper helper;
		internal static IMonitor monitor;
		internal static ITranslationHelper i18n;
		public static API api = new();

		public override void Entry(IModHelper helper)
		{
			config = helper.ReadConfig<Config>();
			ModEntry.helper = helper;
			monitor = Monitor;
			i18n = helper.Translation;

			helper.Events.Content.AssetRequested += LoadAssets;
			helper.Events.GameLoop.GameLaunched += GameLaunched;
			LocalizedContentManager.OnLanguageChange += (c) => helper.GameContent.InvalidateCache(pathLocData);

			AssetPath = "Mods/" + ModManifest.UniqueID;
		}
		[MethodImpl(MethodImplOptions.NoInlining)]
		public void GameLaunched(object sender, GameLaunchedEventArgs ev)
		{
			config.RegisterGMCM(ModManifest);

			var harmony = new Harmony(ModManifest.UniqueID);
			framework.Patches.Patch(harmony);
			framework.ObeliskPatch.Patch(harmony);

			helper.ConsoleCommands.Add(
				"warpnet",
				"Master command for Warp Network mod. Use 'warpnet' or 'warpnet help' to see a list of subcommands.",
				CommandHandler.Main);
			DataPatcher.Init();
			CPIntegration.AddTokens(ModManifest);
		}
		public override object GetApi() => api;
		private void LoadAssets(object _, AssetRequestedEventArgs ev)
		{
			if (ev.NameWithoutLocale.IsEquivalentTo(pathObjectData))
				ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/WarpObjects.json", AssetLoadPriority.Low);
			else if (ev.NameWithoutLocale.IsEquivalentTo(pathLocData))
				ev.LoadFromModFile<Dictionary<string, WarpLocation>>("assets/Destinations.json", AssetLoadPriority.Medium);
			else if (ev.NameWithoutLocale.IsEquivalentTo(pathItemData))
				ev.LoadFromModFile<Dictionary<string, WarpItem>>("assets/WarpItems.json", AssetLoadPriority.Medium);
			else if (ev.NameWithoutLocale.StartsWith(pathIcons))
			{
				var name = ev.NameWithoutLocale.ToString().WithoutPath(pathIcons);
				if (knownIcons.Contains(name))
					ev.LoadFromModFile<Texture2D>($"assets/icons/{name}.png", AssetLoadPriority.Low);
				else
					ev.LoadFrom(() => helper.GameContent.Load<Texture2D>($"{pathIcons}/DEFAULT"), AssetLoadPriority.Low);
			}
		}
	}
}

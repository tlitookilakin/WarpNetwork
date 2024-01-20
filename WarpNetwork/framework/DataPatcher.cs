using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using WarpNetwork.models;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace WarpNetwork.framework
{
	class DataPatcher
	{
		private static readonly string[] DefaultDests = { "farm", "mountain", "beach", "desert", "island" };

		public static Dictionary<string, WarpLocation> ApiLocs = new(StringComparer.OrdinalIgnoreCase);
		public static Dictionary<string, WarpItem> ApiItems = new(StringComparer.OrdinalIgnoreCase);

		// TODO: add data porting
		internal static void Init()
		{
			ModEntry.helper.Events.Content.AssetRequested += AssetRequested;
		}
		internal static void AssetRequested(object _, AssetRequestedEventArgs ev)
		{
			if (ev.NameWithoutLocale.IsEquivalentTo(ModEntry.pathLocData))
				ev.Edit((a) => EditLocations(a.AsDictionary<string, WarpLocation>().Data));
			else if (ev.NameWithoutLocale.IsEquivalentTo(ModEntry.pathItemData))
				ev.Edit((a) => AddApiItems(a.AsDictionary<string, WarpItem>().Data));
			else if (ModEntry.config.MenuEnabled && MapHasWarpStatue(ev.NameWithoutLocale))
				ev.Edit((a) => AddVanillaWarpStatue(a.AsMap(), ev.NameWithoutLocale.ToString()), AssetEditPriority.Late);
		}
		private static bool MapHasWarpStatue(IAssetName name)
		{
			return
					name.IsEquivalentTo("Maps/Beach") ||
					PathUtilities.GetSegments(name.ToString(), 2)[^1].StartsWith("Beach-") ||
					name.IsEquivalentTo("Maps/Island_S") ||
					name.IsEquivalentTo("Maps/Mountain") ||
					name.IsEquivalentTo("Maps/Desert") ||
					name.IsEquivalentTo("Maps/" + Utils.GetFarmMapPath())
				;
		}
		private static void AddApiItems(IDictionary<string, WarpItem> dict)
		{
			foreach (string key in ApiItems.Keys)
				dict[key] = ApiItems[key];
		}
		private static void EditLocations(IDictionary<string, WarpLocation> dict)
		{
			foreach (string key in ApiLocs.Keys)
				dict[key] = ApiLocs[key];

			foreach (string key in DefaultDests)
				if (dict.TryGetValue(key, out var dest))
				{
					Translation label = ModEntry.i18n.Get("dest." + key);

					if (label.HasValue())
						dest.Label = label.ToString();
					dest.Condition = ModEntry.config.WarpsEnabled != WarpEnabled.Never ? "TRUE" : "FALSE";
				}
		}
		private static void AddVanillaWarpStatue(IAssetDataForMap map, string Name)
		{
			Name = PathUtilities.GetSegments(Name)[^1];
			Name = Name == "Island_S" ? "island" : Name.StartsWith("Beach") ? "beach" : Name.ToLowerInvariant();
			string id = Name == Path.GetFileName(Utils.GetFarmMapPath()).ToLowerInvariant() ? "farm" : Name;

			if (!map.Data.Properties.ContainsKey("WarpNetworkEntry"))
			{
				var locs = Utils.GetWarpLocations();
				if (!locs.TryGetValue(id, out var loc))
				{
					ModEntry.monitor.Log($"No destination entry for vanilla location '{id}'; skipping!", LogLevel.Warn);
					return;
				}
				Layer Buildings = map.Data.GetLayer("Buildings");
				if (Buildings is null)
				{
					ModEntry.monitor.Log($"Could not add Warp Network to vanilla location '{id}'; Map is missing Buildings layer", LogLevel.Warn);
				}
				else if (loc is WarpLocation warp)
				{
					if (warp.Position != default)
					{
						Point tilePos = warp.Position;
						if (id == "farm")
						{
							Utils.TryGetActualFarmPoint(ref tilePos, map.Data, Name);
						}
						var spot = new Location(tilePos.X, tilePos.Y).Above;

						ModEntry.monitor.Log($"Adding access point for destination '{id}' @ {spot.X}, {spot.Y}");

						Tile tile = Buildings.Tiles[spot];
						if (tile is null)
							ModEntry.monitor.Log($"No tile in building layer, could not add access point: '{id}' @ {spot.X}, {spot.Y}", LogLevel.Warn);
						else
							tile.Properties["Action"] = "WarpNetwork " + id;
					}
					else
					{
						ModEntry.monitor.Log($"Could not add Warp Network to vanilla location '{id}'; Coordinates are outside map bounds.", LogLevel.Warn);
					}
				}
				else
				{
					ModEntry.monitor.Log($"Could not add warp stature to Vanilla destination '{id}' because it's using a custom handler!", LogLevel.Warn);
				}
			}
		}
	}
}

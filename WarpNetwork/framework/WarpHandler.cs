﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using WarpNetwork.api;
using WarpNetwork.models;
using WarpNetwork.ui;

namespace WarpNetwork.framework
{
	class WarpHandler
	{
		internal static void Init()
		{
			GameLocation.RegisterTileAction("warpnetwork", WarpNetAction);
			GameLocation.RegisterTileAction("warpnetworkto", WarpToAction);
			GameLocation.RegisterTouchAction("warpnetworkto", (w, s, f, l) => DirectWarp(s.Length is 0 ? string.Empty : s[0], w, f));
			ModEntry.helper.Events.GameLoop.DayEnding += Cleanup;
			ModEntry.helper.Events.GameLoop.ReturnedToTitle += Cleanup;
		}
		private static void Cleanup(object sender, object ev)
		{
			ReturnHandler.Instance.ClearReturnPoint();
		}
		private static bool WarpNetAction(GameLocation where, string[] split, Farmer who, Point tile)
		{
			var locationOfTotem = split.Length < 2 ? string.Empty : split[1];
			ShowWarpMenu(where, who, locationOfTotem);
			return true;
		}
		private static bool WarpToAction(GameLocation where, string[] split, Farmer who, Point tile)
		{
            var id = split.Length < 2 ? string.Empty : split[1];
            DirectWarp(id, where, who);
			return true;
		}
		public static void ShowWarpMenu(GameLocation where, Farmer who, string exclude = "", bool consume = false)
		{
			ModEntry.monitor.Log($"Activating menu; exclude: {exclude}, consume: {consume}");
			if (!ModEntry.config.MenuEnabled)
			{
				ShowFailureText();
				ModEntry.monitor.Log("Warp menu is disabled in config, menu not displayed.");
				return;
			}

			List<IWarpNetAPI.IDestinationHandler> dests = new();
			var locs = Utils.GetWarpLocations();

			if (locs.ContainsKey(exclude))
			{
				if (!locs.IsAccessible(exclude, where, who) && !ModEntry.config.AccessFromDisabled)
				{
					ShowFailureText();
					ModEntry.monitor.Log("Access from locked locations is disabled, menu not displayed.");
					return;
				}
			}

			if (exclude.Equals("_wand", StringComparison.OrdinalIgnoreCase) && ModEntry.config.WandReturnEnabled && ReturnHandler.Instance.HasReturnPoint)
			{
				dests.Add(ReturnHandler.Instance);
			}

			foreach ((string id, var loc) in locs)
			{
				if (
					loc.IsVisible(where, who) &&
					(
						// forced
						exclude.Equals("_force", StringComparison.OrdinalIgnoreCase) ||

						// not where we're warping from, and is accessible
						!id.Equals(exclude, StringComparison.OrdinalIgnoreCase) && locs.IsAccessible(id, where, who) ||

						// always show the farm if it's the wand
						id.Equals("farm", StringComparison.OrdinalIgnoreCase) && exclude.Equals("_wand", StringComparison.OrdinalIgnoreCase)
					)
				)
				{
					if (loc is not WarpLocation warploc || Game1.getLocationFromName(warploc.Location) is not null)
						dests.Add(locs[id]);
					else
						ModEntry.monitor.Log("Invalid Location name '" + warploc.Location + "'; skipping entry.", LogLevel.Warn);
				}
			}

			if (dests.Count is 0)
			{
				ModEntry.monitor.Log("No valid warp destinations, menu not displayed.");
				ShowFailureText();
				return;
			}

			Game1.activeClickableMenu = new WarpMenu(dests);
		}
		internal static void ShowFailureText()
			=> Game1.drawObjectDialogue(Game1.parseText(ModEntry.i18n.Get("ui-fail")));

		internal static void ShowFestivalNotReady()
			=> Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:Game1.cs.2973"));

		public static bool DirectWarp(string prop, GameLocation where, Farmer who)
		{
			var args = prop.Split(' ');
			if (args.Length > 0)
				return DirectWarp(args[0], args.Length > 1, where, who);

			ModEntry.monitor.Log("Warning! Map '" + Game1.currentLocation.Name + "' has invalid WarpNetworkTo property! Location MUST be specified!", LogLevel.Warn);
			return false;
		}

		public static bool DirectWarp(string location, bool force, GameLocation where, Farmer who)
		{
			if (location is not null)
				if (Utils.GetWarpLocations().TryGetValue(location, out var loc) &&
					(force || loc.IsAccessible(where, who)) &&
					ActivateWarp(loc, where, who)
				)
					return true;
				else
					ModEntry.monitor.Log($"Warp to '{location}' failed: warp network location not registered with that name", LogLevel.Warn);
			else
				ModEntry.monitor.Log("Destination is null! Cannot warp!", LogLevel.Error);

			ShowFailureText();
			return false;
		}
		internal static bool ActivateWarp(IWarpNetAPI.IDestinationHandler handler, GameLocation where, Farmer who)
		{
			var name = Game1.currentLocation.NameOrUniqueName;
			var tile = Game1.player.TilePoint;


			bool r = handler.Activate(where, who);

			foreach (var loc in Utils.CustomLocs.Values)
				loc.AfterWarp(name, tile, handler);
			return r;
		}
	}
}

using StardewValley;
using SObject = StardewValley.Object;
using System;
using HarmonyLib;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;
using WarpNetwork.models;
using StardewValley.Tools;

namespace WarpNetwork.framework
{
	internal class Patches
	{
		private static readonly Dictionary<string, Point> ObeliskTargets = new()
		{
			{ "Farm", new Point(48, 7) },
			{ "IslandSouth", new Point(11, 11) },
			{ "Mountain", new Point(31, 20) },
			{ "Beach", new Point(20, 4) },
			{ "Desert", new Point(35, 43) }
		};

		internal static void Patch(Harmony harmony)
		{
			// FIX: Override Wand.DoFunction instead.
			harmony.Patch(
				typeof(Wand).GetMethod(nameof(Wand.DoFunction)), 
				new(typeof(Patches), nameof(WandDoFunctionPrefix)));


			harmony.Patch(
				typeof(SObject).GetMethod(nameof(SObject.checkForAction)), 
				new(typeof(Patches), nameof(ActionPrefix)));
			harmony.Patch(
				typeof(Building).GetMethod(nameof(Building.PerformObeliskWarp)),
				new(typeof(Patches), nameof(MoveTarget))
			);
		}

		private static bool WandDoFunctionPrefix(GameLocation location, SObject __instance)
		{
			if (__instance.isTemporarilyInvisible)
				return true;

			if (Game1.eventUp || Game1.isFestival() &&
				Game1.fadeToBlack || Game1.player.swimming.Value &&
				Game1.player.bathingClothes.Value || Game1.player.onBridge.Value)
				return true;

			if (!ItemHandler.TryUseWand(Game1.player))
				return true;

			return false;
		}

		private static bool ActionPrefix(Farmer who, bool justCheckingForActivity, SObject __instance, ref bool __result)
		{
			if (__instance.isTemporarilyInvisible || who is null)
				return true;

			if (!ItemHandler.ActivateObject(__instance, justCheckingForActivity, who))
				return true;

			__result = true;
			return false;
		}

		private static bool MoveTarget(Building __instance, ref string destination, ref int warp_x, ref int warp_y, Farmer who)
		{
			if (who == Game1.player)
			{
				string Name = destination;
				if (ModEntry.config.PatchObelisks)
				{
					if (ObeliskTargets.TryGetValue(Name, out var dest) && new Point(warp_x, warp_y) == dest)
					{
						string target = Name is "IslandSouth" ? "island" : Name;
						if (Utils.GetWarpLocations().TryGetValue(target, out var loc))
						{
							if (loc is WarpLocation warp)
							{
								var tile = warp.GetLandingPoint();
								warp_x = tile.X;
								warp_y = tile.Y;
								destination = warp.Location.Trim();
							}
							else
							{
								WarpHandler.ActivateWarp(loc, __instance.GetParentLocation(), who);
								return false;
							}
						}
					}
				}
			}

			return true;
		}
	}
}

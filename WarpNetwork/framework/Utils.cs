using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using WarpNetwork.api;
using WarpNetwork.models;
using xTile;
using DColor = System.Drawing.Color;

namespace WarpNetwork.framework
{
	static class Utils
	{
        private static readonly string[] VanillaMapNames =
        {
            "Farm","Farm_Fishing","Farm_Foraging","Farm_Mining","Farm_Combat","Farm_FourCorners","Farm_Island"
        };
        
		public static Dictionary<string, IWarpNetAPI.IDestinationHandler> CustomLocs =
			new(StringComparer.OrdinalIgnoreCase) { {"_return", ReturnHandler.Instance } };

		public static Point GetTargetTile(GameLocation where, Point target = default)
		{
			Point tile = target;
			if (where is Farm)
			{
				tile = GetFarmTile(where);
			}
			else if (target == default)
			{
				if (!DataLoader.Locations(Game1.content).TryGetValue(where.Name, out var data) || !data.DefaultArrivalTile.HasValue)
					return default;

				tile = data.DefaultArrivalTile.Value;
			}

			if (where.TryGetMapPropertyAs("WarpNetworkEntry", out Point prop, false))
				tile = prop;

			return tile;
		}

		public static Point GetFarmTile(GameLocation farm)
		{
			Point tile = Game1.GetFarmTypeID() switch
			{
				// four corners
				"5" => new(48, 39),

				// beach
				"6" => new(82, 29),

				// everything else
				_ => new(48, 7),
			};

			if (farm.TryGetMapPropertyAs("WarpTotemEntry", out Point prop, false))
				tile = prop;

			return tile;
		}

		public static Dictionary<string, IWarpNetAPI.IDestinationHandler> GetWarpLocations()
		{
			Dictionary<string, IWarpNetAPI.IDestinationHandler> ret = new Dictionary<string, IWarpNetAPI.IDestinationHandler>(StringComparer.OrdinalIgnoreCase);
			foreach ((var key, var value) in ModEntry.helper.GameContent.Load<Dictionary<string, WarpLocation>>(ModEntry.AssetPath + "/Destinations"))
			{
				ret[key] = value;
			}

			foreach ((var key, var value) in CustomLocs)
				ret[key] = value;

			return ret;
		}

		public static Dictionary<string, WarpItem> GetWarpItems()
			=> new(
				ModEntry.helper.GameContent.Load<Dictionary<string, WarpItem>>(ModEntry.AssetPath + "/Totems"), 
				StringComparer.OrdinalIgnoreCase
			);

		public static Dictionary<string, WarpItem> GetWarpObjects()
			=> new(
				ModEntry.helper.GameContent.Load<Dictionary<string, WarpItem>>(ModEntry.AssetPath + "/PlacedTotems"),
				StringComparer.OrdinalIgnoreCase
			);

		public static string WithoutPath(this string path, string prefix)
			=> PathUtilities.GetSegments(path, PathUtilities.GetSegments(prefix).Length + 1)[^1];

		public static bool IsAccessible(this IDictionary<string, IWarpNetAPI.IDestinationHandler> dict, string id, GameLocation where, Farmer who)
			=> ModEntry.config.OverrideEnabled switch
			{
				WarpEnabled.Never => false,
				WarpEnabled.Always => true,
				WarpEnabled.Default =>
					dict.TryGetValue(id, out var loc) &&
					loc.IsAccessible(where, who),
				_ => false
			};

		public static string ToLocalLocale(this IModHelper helper, string Locale)
		{
			var split = Locale.LastIndexOf('-');
			var broad = split < 0 ? null : Locale[..split];

			var fname = Path.Join(helper.DirectoryPath, "i18n", Locale + ".json");

			if (File.Exists(fname))
				return Locale;

			fname = Path.Join(helper.DirectoryPath, "i18n", broad + ".json");

			if (File.Exists(fname))
				return broad;

			return "default";
		}

		public static TemporaryAnimatedSprite WithItem(this TemporaryAnimatedSprite sprite, Item item)
		{
			sprite.CopyAppearanceFromItemId(item.QualifiedItemId);
			return sprite;
		}

		public static bool TryParseColor(this string str, out Color color)
		{
			color = Color.Transparent;

			if (str is null || str.Length == 0)
				return false;

			DColor c = DColor.FromName(str);
			if (c.ToArgb() != 0)
			{
				color = new(c.R, c.G, c.B, c.A);
				return true;
			}

			ReadOnlySpan<char> s = str.AsSpan();
			if (s[0] == '#')
			{
				if (s.Length <= 3)
					return false;

				if (s.Length > 6)
				{
					if (int.TryParse(s[1..3], NumberStyles.HexNumber, null, out int r) &&
						int.TryParse(s[3..5], NumberStyles.HexNumber, null, out int g) &&
						int.TryParse(s[5..7], NumberStyles.HexNumber, null, out int b))
					{
						if (s.Length > 8 && int.TryParse(s[7..9], NumberStyles.HexNumber, null, out int a))
							color = new(r, g, b, a);
						else
							color = new(r, g, b);
						return true;
					}
				}
				else
				{
					if (int.TryParse($"{s[1]}{s[1]}", NumberStyles.HexNumber, null, out int r) &&
						int.TryParse($"{s[2]}{s[2]}", NumberStyles.HexNumber, null, out int g) &&
						int.TryParse($"{s[3]}{s[3]}", NumberStyles.HexNumber, null, out int b))
					{
						if (s.Length > 4 && int.TryParse($"{s[4]}{s[4]}", NumberStyles.HexNumber, null, out int a))
							color = new(r, g, b, a);
						else
							color = new(r, g, b);
						return true;
					}
				}
			}
			else
			{
				string[] vals = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
				if (vals.Length > 2)
				{
					if (int.TryParse(vals[0], out int r) &&
						int.TryParse(vals[1], out int g) &&
						int.TryParse(vals[2], out int b))
					{
						if (vals.Length > 3 && int.TryParse(vals[3], out int a))
							color = new Color(r, g, b, a);
						else
							color = new Color(r, g, b);
						return true;
					}
				}
			}
			return false;
		}

		internal static void AddQuickBool(this IGMCMAPI api, object inst, IManifest manifest, string prop)
		{
			var p = inst.GetType().GetProperty(prop);
			var cfname = prop;
			api.AddBoolOption(manifest,
				p.GetGetMethod().CreateDelegate<Func<bool>>(inst),
				p.GetSetMethod().CreateDelegate<Action<bool>>(inst),
				() => ModEntry.i18n.Get($"config.{cfname}.label"),
				() => ModEntry.i18n.Get($"config.{cfname}.desc")
			);
		}

		internal static void AddQuickEnum<TE>(this IGMCMAPI api, object inst, IManifest manifest, string prop) where TE : Enum
		{
			var p = inst.GetType().GetProperty(prop);

			// It looks to me like at some point in the past (like prior to the update to 1.6), somebody did a
			//  code-cleanup and renamed the property from 'WarpsEnabled' to 'OverrideEnabled' not realizing that
			//  the property name was also tied in to the localization labels.  This is the least-churn way of
			//  making it work.  I'm not so sure that "Fixing it properly" would entail the wider change, as
			//  to my mind "properly" would be a change to prevent this sort of thing from recurring.
			var cfname = prop == "OverrideEnabled" ? "WarpsEnabled" : prop;

			var tenum = typeof(TE);
			var tname = tenum.Name;
			api.AddTextOption(manifest,
				() => p.GetValue(inst).ToString(),
				(s) => p.SetValue(inst, (TE)Enum.Parse(tenum, s)),
				() => ModEntry.i18n.Get($"config.{cfname}.label"),
				() => ModEntry.i18n.Get($"config.{cfname}.desc"),
				Enum.GetNames(tenum),
				(s) => ModEntry.i18n.Get($"config.{tname}.{s}")
			);
		}

        public static string GetFarmMapPath()
        {
            if (Game1.whichFarm < 0)
            {
                ModEntry.monitor.Log("Something is wrong! Game1.whichfarm does not contain a valid value!", LogLevel.Warn);
                return "";
            }

            if (Game1.whichFarm < 7)
                return VanillaMapNames[Game1.whichFarm];

            if (Game1.whichModFarm is null)
            {
                ModEntry.monitor.Log("Something is wrong! Custom farm indicated, but Game1.whichModFarm is null!", LogLevel.Warn);
                return "";
            }

            return Game1.whichModFarm.MapName;
        }

        public static void TryGetActualFarmPoint(ref Point Position, Map map = null, string filename = null)
        {
            map ??= Game1.getFarm().Map;

            switch (GetFarmType(filename))
            {
                //four corners
                case 5:
                    Position = new(48, 39);
                    break;

                //beach
                case 6:
                    Position = new(82, 29);
                    break;
            }
            TryGetMapPropertyPosition(map, "WarpTotemEntry", ref Position);
        }
        private static readonly Dictionary<string, int> FarmTypeMap = new()
        {
            { "farm", 0 },
            { "farm_fishing", 1 },
            { "farm_foraging", 2 },
            { "farm_mining", 3 },
            { "farm_combat", 4 },
            { "farm_fourcorners", 5 },
            { "farm_island", 6 }
        };
        public static int GetFarmType(string filename)
		    => filename is null ? Game1.whichFarm : FarmTypeMap.TryGetValue(filename, out var type) ? type : 0;
        public static bool TryGetMapPropertyPosition(Map map, string property, ref Point position)
        {
            if (!map.Properties.TryGetValue(property, out var v))
                return false;

            string prop = (string)v;

            string[] args = prop.Split(' ');
            if (args.Length < 2)
                return false;

            if (int.TryParse(args[0], out int x) && int.TryParse(args[1], out int y))
                position = new(x, y);
            else
                return false;

            return true;
        }
    }
}

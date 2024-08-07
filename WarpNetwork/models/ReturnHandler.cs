﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using WarpNetwork.api;
using WarpNetwork.framework;

namespace WarpNetwork.models
{
    public class ReturnHandler : IWarpNetAPI.IDestinationHandler
    {
        public static readonly ReturnHandler Instance = new();

        private readonly PerScreen<Point> targetTile = new();
        private readonly PerScreen<string> targetLocation = new();

        public bool HasReturnPoint => this.targetLocation.Value is not null && Game1.getLocationFromName(this.targetLocation.Value) is not null;

        public void ClearReturnPoint()
        {
            this.targetTile.ResetAllScreens();
            this.targetLocation.ResetAllScreens();
        }

        public void SetReturnLocation()
        {
            this.targetLocation.Value = Game1.currentLocation.NameOrUniqueName;
            this.targetTile.Value = Game1.player.TilePoint;
        }

        public string Label
            => ModEntry.i18n.Get("dest.return");

        public Texture2D Icon
            => ModEntry.helper.GameContent.Load<Texture2D>(ModEntry.AssetPath + "/Icons/RETURN");

        public bool Activate(GameLocation location, Farmer who)
        {
            if (!IsAccessible(location, who))
                return false;

            (int x, int y) = targetTile.Value;
            string loc = targetLocation.Value;

            //MUST copy to preserve. warp is called on delay and values may change.
            API.api.DoWarpEffects(() => Game1.warpFarmer(loc, x, y, false), who, location);

            targetLocation.Value = null;
            return true;
        }

        public bool IsAccessible(GameLocation location, Farmer who)
        {
            return
                Game1.CurrentEvent is null &&
                targetLocation.Value is not null &&
                ModEntry.config.WandReturnEnabled;
        }

        public void AfterWarp(string name, Point tile, IWarpNetAPI.IDestinationHandler handler)
        {
            if (handler == this || name is "Temp")
                return;

            targetTile.Value = tile;
            targetLocation.Value = name;
        }

        public bool IsVisible(GameLocation location, Farmer who)
        {
            return IsAccessible(location, who);
        }
    }
}

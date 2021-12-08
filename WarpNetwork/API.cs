using System;
using System.Collections.Generic;
using System.Linq;

namespace WarpNetwork
{
    public class API : IWarpNetAPI
    {
        public void AddCustomDestinationHandler(string ID, IWarpNetHandler handler)
        {
            if (WarpHandler.CustomLocs.ContainsKey(ID))
            {
                WarpHandler.CustomLocs.Remove(ID);
            }
            WarpHandler.CustomLocs.Add(ID, handler);
        }
        public bool CanWarpTo(string ID)
        {
            if (WarpHandler.CustomLocs.ContainsKey(ID))
            {
                return WarpHandler.CustomLocs[ID].GetEnabled();
            }
            Dictionary<string, WarpLocation> dict = Utils.GetWarpLocations();
            if (dict.ContainsKey(ID))
            {
                return dict[ID].Enabled;
            }
            return false;
        }
        public string[] GetItems()
        {
            return Utils.GetWarpItems().Keys.ToArray();
        }
        public string[] GetDestinations()
        {
            return Utils.GetWarpLocations().Keys.Concat(WarpHandler.CustomLocs.Keys).ToArray();
        }
        public bool DestinationExists(string ID)
        {
            return GetDestinations().Contains(ID, StringComparer.OrdinalIgnoreCase);
        }
        public bool DestinationIsCustomHandler(string ID)
        {
            return WarpHandler.CustomLocs.ContainsKey(ID);
        }
        public void RemoveCustomDestinationHandler(string ID)
        {
            WarpHandler.CustomLocs.Remove(ID);
        }
        public void ShowWarpMenu(bool force = false)
        {
            ShowWarpMenu(force ? "_force" : "");
        }
        public void ShowWarpMenu(string Exclude)
        {
            WarpHandler.ShowWarpMenu(Exclude);
        }
        public bool WarpTo(string ID)
        {
            return WarpHandler.DirectWarp(ID, true);
        }
    }
}

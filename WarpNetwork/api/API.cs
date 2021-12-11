using System;
using System.Collections.Generic;
using System.Linq;
using WarpNetwork.models;

namespace WarpNetwork.api
{
    public class API : IWarpNetAPI
    {
        public bool AddCustomDestinationHandler(string ID, object handler)
        {
            if (WarpHandler.CustomLocs.ContainsKey(ID))
            {
                WarpHandler.CustomLocs.Remove(ID);
            }
            IWarpNetHandler h = Utils.WrapHandlerObject(handler);
            if (h != null)
            {
                WarpHandler.CustomLocs.Add(ID, h);
                return true;
            }
            return false;
        }
        public void AddCustomDestinationHandler(string ID, Func<bool> getEnabled, Func<string> getLabel, Func<string> getIconName, Action warp)
        {
            if (WarpHandler.CustomLocs.ContainsKey(ID))
            {
                WarpHandler.CustomLocs.Remove(ID);
            }
            WarpHandler.CustomLocs.Add(ID, new WarpNetHandler(getEnabled, getIconName, getLabel, warp));
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

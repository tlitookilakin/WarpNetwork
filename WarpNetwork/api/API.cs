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
            if (Utils.CustomLocs.ContainsKey(ID))
            {
                Utils.CustomLocs.Remove(ID);
            }
            IWarpNetHandler h = Utils.WrapHandlerObject(handler);
            if (h != null)
            {
                Utils.CustomLocs.Add(ID, h);
                return true;
            }
            return false;
        }
        public void AddCustomDestinationHandler(string ID, Func<bool> getEnabled, Func<string> getLabel, Func<string> getIconName, Action warp)
        {
            if (Utils.CustomLocs.ContainsKey(ID))
            {
                Utils.CustomLocs.Remove(ID);
            }
            Utils.CustomLocs.Add(ID, new WarpNetHandler(getEnabled, getIconName, getLabel, warp));
        }
        public bool CanWarpTo(string ID)
        {
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
            return Utils.GetWarpLocations().Keys.ToArray();
        }
        public bool DestinationExists(string ID)
        {
            return GetDestinations().Contains(ID, StringComparer.OrdinalIgnoreCase);
        }
        public bool DestinationIsCustomHandler(string ID)
        {
            return Utils.CustomLocs.ContainsKey(ID);
        }
        public void RemoveCustomDestinationHandler(string ID)
        {
            Utils.CustomLocs.Remove(ID);
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

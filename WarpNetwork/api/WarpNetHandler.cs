using System;

namespace WarpNetwork.api
{
    class WarpNetHandler : IWarpNetHandler
    {
        private readonly Func<bool> getEnabled;
        private readonly Func<string> getIconName;
        private readonly Func<string> getLabel;
        private readonly Action warp;
        internal WarpNetHandler(Func<bool> enabled, Func<string> icon, Func<string> label, Action warp)
        {
            getEnabled = enabled;
            getIconName = icon;
            getLabel = label;
            this.warp = warp;
        }
        public bool GetEnabled()
        {
            return getEnabled();
        }
        public string GetIconName()
        {
            return getIconName();
        }
        public string GetLabel()
        {
            return getLabel();
        }
        public void Warp()
        {
            warp();
        }
    }
}

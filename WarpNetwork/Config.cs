using StardewModdingAPI;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace WarpNetwork
{
    enum WarpEnabled
    {
        AfterObelisk,
        Always,
        Never
    }
    class Config
    {
        public WarpEnabled VanillaWarpsEnabled { get; set; } = WarpEnabled.AfterObelisk;
        public WarpEnabled FarmWarpEnabled { get; set; } = WarpEnabled.AfterObelisk;
        public bool AccessFromDisabled { get; set; } = false;
        public bool AccessFromWand { get; set; } = false;
        public bool PatchObelisks { get; set; } = true;
        public bool MenuEnabled { get; set; } = true;
        public bool WarpCancelEnabled { get; set; } = false;
        public bool WandReturnEnabled { get; set; } = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ObeliskCheckRequired()
            => VanillaWarpsEnabled == WarpEnabled.AfterObelisk || FarmWarpEnabled == WarpEnabled.AfterObelisk;
        internal string AsText()
        {
            StringBuilder sb = new();
            sb.AppendLine().AppendLine("Config:");
            sb.Append("\tVanillaWarpsEnabled: ").AppendLine(VanillaWarpsEnabled.ToString());
            sb.Append("\tFarmWarpEnabled: ").AppendLine(FarmWarpEnabled.ToString());
            sb.Append("\tAccessFromDisabled: ").AppendLine(AccessFromDisabled.ToString());
            sb.Append("\tAccessFromWand: ").AppendLine(AccessFromWand.ToString());
            sb.Append("\tPatchObelisks: ").AppendLine(PatchObelisks.ToString());
            sb.Append("\tMenuEnabled: ").AppendLine(MenuEnabled.ToString());
            return sb.ToString();
        }
    }
}

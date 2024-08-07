﻿using StardewModdingAPI;
using System.Text;
using WarpNetwork.api;
using WarpNetwork.framework;

namespace WarpNetwork
{
	enum WarpEnabled
	{
		Default,
		Always,
		Never
	}
	class Config
	{
		public WarpEnabled OverrideEnabled { get; set; } = WarpEnabled.Default;
		public bool AccessFromDisabled { get; set; } = false;
		public bool AccessFromWand { get; set; } = true;
		public bool PatchObelisks { get; set; } = true;
		public bool MenuEnabled { get; set; } = true;
		public bool WarpCancelEnabled { get; set; } = false;
		public bool WandReturnEnabled { get; set; } = true;
		internal string AsText()
		{
			StringBuilder sb = new();
			sb.AppendLine().AppendLine("Config:");
			sb.Append("\tVanillaWarpsEnabled: ").AppendLine(OverrideEnabled.ToString());
			sb.Append("\tAccessFromDisabled: ").AppendLine(AccessFromDisabled.ToString());
			sb.Append("\tAccessFromWand: ").AppendLine(AccessFromWand.ToString());
			sb.Append("\tPatchObelisks: ").AppendLine(PatchObelisks.ToString());
			sb.Append("\tMenuEnabled: ").AppendLine(MenuEnabled.ToString());
			return sb.ToString();
		}

		internal void RegisterGMCM(IManifest manifest)
		{
			//helper.GameContent.InvalidateCache(pathLocData)

			var gmcm = ModEntry.helper.ModRegistry.GetApi<IGMCMAPI>("spacechase0.GenericModConfigMenu");
			if (gmcm is null)
			{
				return;
			}

			gmcm.Register(manifest, Reset, Save);

			gmcm.AddQuickEnum<WarpEnabled>(this, manifest, nameof(OverrideEnabled));
			gmcm.AddQuickBool(this, manifest, nameof(AccessFromDisabled));
			gmcm.AddQuickBool(this, manifest, nameof(AccessFromWand));
			gmcm.AddQuickBool(this, manifest, nameof(PatchObelisks));
			gmcm.AddQuickBool(this, manifest, nameof(MenuEnabled));
			gmcm.AddQuickBool(this, manifest, nameof(WarpCancelEnabled));
			gmcm.AddQuickBool(this, manifest, nameof(WandReturnEnabled));
		}
		private void Save()
		{
			ModEntry.helper.WriteConfig(this);
		}
		private void Reset()
		{
			OverrideEnabled = WarpEnabled.Default;
			AccessFromDisabled = false;
			AccessFromWand = true;
			PatchObelisks = true;
			MenuEnabled = true;
			WarpCancelEnabled = false;
			WandReturnEnabled = true;
		}
	}
}

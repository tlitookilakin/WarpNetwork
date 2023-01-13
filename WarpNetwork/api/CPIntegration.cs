using StardewModdingAPI;

namespace WarpNetwork.api
{
	class CPIntegration
	{
		public static void AddTokens(IManifest manifest)
		{
			if (!ModEntry.helper.ModRegistry.IsLoaded("pathoschild.ContentPatcher"))
				return;
			var api = ModEntry.helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
			api.RegisterToken(manifest, "MenuEnabled", () => new[] { ModEntry.config.MenuEnabled.ToString() });
		}
	}
}

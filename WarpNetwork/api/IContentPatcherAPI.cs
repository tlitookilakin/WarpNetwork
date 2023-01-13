using StardewModdingAPI;
using System;
using System.Collections.Generic;

namespace WarpNetwork.api
{
	public interface IContentPatcherAPI
	{
		void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);
		void RegisterToken(IManifest mod, string name, object token);
	}
}

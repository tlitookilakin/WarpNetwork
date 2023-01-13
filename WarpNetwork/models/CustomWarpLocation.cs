using WarpNetwork.api;

namespace WarpNetwork.models
{
	class CustomWarpLocation : WarpLocation
	{
		public readonly IWarpNetHandler handler;
		public CustomWarpLocation(IWarpNetHandler handler) : base()
		{
			this.handler = handler;
		}
		public override string Label => handler.GetLabel();
		public override string Icon => handler.GetIconName();
		public override bool Enabled => handler.GetEnabled();
	}
}

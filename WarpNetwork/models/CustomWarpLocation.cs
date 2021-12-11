using WarpNetwork.api;

namespace WarpNetwork.models
{
    class CustomWarpLocation : WarpLocation
    {
        private readonly IWarpNetHandler handler;
        public CustomWarpLocation(IWarpNetHandler handler) : base()
        {
            this.handler = handler;
        }
        public override string Label
        {
            get { return handler.GetLabel(); }
        }
        public override string Icon { 
            get { return handler.GetIconName(); }
        }
        public override bool Enabled
        {
            get { return handler.GetEnabled(); }
        }
    }
}

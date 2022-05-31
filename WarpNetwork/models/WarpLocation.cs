using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using xTile.Dimensions;

namespace WarpNetwork.models
{
    class WarpLocation
    {
        private Texture2D cachedIcon = null;

        public string Location { set; get; }
        public int X { set; get; } = 0;
        public int Y { set; get; } = 1;
        public virtual bool Enabled { set; get; } = false;
        public virtual string Label { set; get; }
        public bool OverrideMapProperty { set; get; } = false;
        public bool AlwaysHide { get; set; } = false;
        public virtual string Icon { set; get; } = "";
        public Texture2D IconTex
        {
            get
            {
                if (cachedIcon == null)
                {
                    try
                    {
                        cachedIcon = ModEntry.helper.GameContent.Load<Texture2D>("Data/WarpNetwork/Icons/" + Icon);
                    }
                    catch (ContentLoadException)
                    {
                        cachedIcon = ModEntry.helper.GameContent.Load<Texture2D>("Data/WarpNetwork/Icons/DEFAULT");
                    }
                }
                return cachedIcon;
            }
        }

        public Location CoordsAsLocation() => new Location(X, Y);
    }
}

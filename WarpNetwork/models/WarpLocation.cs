using xTile.Dimensions;

namespace WarpNetwork.models
{
    class WarpLocation
    {
        public string Location { set; get; }
        public int X { set; get; } = 0;
        public int Y { set; get; } = 1;
        virtual public bool Enabled { set; get; } = false;
        virtual public string Label { set; get; }
        public bool OverrideMapProperty { set; get; } = false;
        public bool AlwaysHide { get; set; } = false;
        virtual public string Icon { set; get; } = "";

        public Location CoordsAsLocation()
        {
            return new Location(X, Y);
        }
    }
}

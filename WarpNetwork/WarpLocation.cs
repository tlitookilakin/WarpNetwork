using xTile.Dimensions;

namespace WarpNetwork
{
    class WarpLocation
    {
        public string Location { set; get; }
        public int X { set; get; } = 0;
        public int Y { set; get; } = 1;
        public bool Enabled { set; get; } = false;
        public string Label { set; get; }
        public bool OverrideMapProperty { set; get; } = false;
        public bool AlwaysHide { get; set; } = false;

        public Location CoordsAsLocation()
        {
            return new Location(X, Y);
        }
    }
}

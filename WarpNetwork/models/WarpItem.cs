

namespace WarpNetwork.models
{
    class WarpItem
    {
        public string Destination { set; get; }
        public bool IgnoreDisabled { set; get; } = false;
        public string Color { set; get; } = "#ffffff";
        public bool Consume { get; set; } = true;
    }
}

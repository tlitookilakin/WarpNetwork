

namespace WarpNetwork
{
    public interface IWarpNetAPI
    {
        void AddCustomDestinationHandler(string ID, IWarpNetHandler handler);
        void RemoveCustomDestinationHandler(string ID);
        bool CanWarpTo(string ID);
        bool DestinationExists(string ID);
        bool DestinationIsCustomHandler(string ID);
        bool WarpTo(string ID);
        void ShowWarpMenu(bool Force = false);
        void ShowWarpMenu(string Exclude);
        string[] GetDestinations();
        string[] GetItems();
    }
}

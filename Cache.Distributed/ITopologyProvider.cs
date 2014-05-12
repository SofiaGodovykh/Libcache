namespace Kontur.Cache.Distributed
{
    public interface ITopologyProvider
    {
        TopologyRing GetTopology(string name);
    }
}
using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.EngineInterfaces
{
    public interface IMapCore : IMap
    {
        Point CoercePointOntoMap(Point p);
        void AddMapItem(IMapObjectCore item);
        bool RemoveMapItem(IMapObjectCore item);
    }
}

using Magecrawl.Interfaces;
using Magecrawl.Utilities;

namespace Magecrawl.EngineInterfaces
{
    public interface IFOVManager
    {
        bool VisibleSingleShot(IMapCore map, Point viewPoint, int viewableDistance, Point pointWantToView);
    }
}

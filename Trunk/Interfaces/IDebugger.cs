using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Interfaces
{
    public interface IDebugger
    {
        // Debugging calls
        bool[,] PlayerMoveableToEveryPoint();
        Dictionary<ICharacter, List<Point>> CellsInAllMonstersFOV();
        object DebugRequest(string request, object argument);
        List<Point> CellsInPlayersFOV();
    }
}

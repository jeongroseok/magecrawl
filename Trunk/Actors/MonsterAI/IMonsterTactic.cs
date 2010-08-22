using Magecrawl.Utilities;
using Magecrawl.EngineInterfaces;

namespace Magecrawl.Actors.MonsterAI
{
    internal interface IMonsterTactic
    {
        bool CouldUseTactic(IGameEngineCore engine, Monster monster);
        bool UseTactic(IGameEngineCore engine, Monster monster);

        void NewTurn(Monster monster);
        void SetupAttributesNeeded(Monster monster);

        bool NeedsPlayerLOS
        {
            get;
        }
    }
}

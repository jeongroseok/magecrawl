using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    internal interface IMonsterTactic
    {
        bool CouldUseTactic(CoreGameEngine engine, Monster monster);
        bool UseTactic(CoreGameEngine engine, Monster monster);

        void NewTurn(Monster monster);
        void SetupAttributesNeeded(Monster monster);

        bool NeedsPlayerLOS
        {
            get;
        }
    }
}

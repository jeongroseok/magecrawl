using Magecrawl.Utilities;

namespace Magecrawl.GameEngine.Actors.MonsterAI
{
    internal interface IMonsterTactic
    {
        // On a 1(use always)-10(use just before randomly wandering) when should we use this skill if we can
        int Priority
        {
            get;
        }

        bool CouldUseTactic(CoreGameEngine engine, Monster monster);
        void UseTactic(CoreGameEngine engine);

        void SetupAttributesNeeded();
    }
}

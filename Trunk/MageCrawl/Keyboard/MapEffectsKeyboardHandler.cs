using System.Collections.Generic;
using libtcod;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class MapEffectsKeystrokeHandler : BaseKeystrokeHandler
    {
        public MapEffectsKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object request)
        {
            m_gameInstance.SendPaintersRequest((RequestBase)request);
            m_gameInstance.UpdatePainters();
        }
    }
}

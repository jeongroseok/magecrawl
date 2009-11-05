using System.Collections.Generic;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class MapEffectsKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;

        public MapEffectsKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            RequestBase request = (RequestBase)objOne;
            m_gameInstance.SendPaintersRequest(request);
            m_gameInstance.UpdatePainters();
        }
    }
}

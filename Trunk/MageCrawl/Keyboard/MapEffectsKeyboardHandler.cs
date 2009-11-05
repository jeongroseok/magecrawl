using System.Collections.Generic;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Utilities;
using libtcodWrapper;

namespace Magecrawl.Keyboard
{
    internal delegate void OnEffectDone();

    internal class MapEffectsKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;
        private OnEffectDone m_onDoneDelegate;

        public MapEffectsKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object objOne, object objTwo, object objThree, object objFour)
        {
            string effectType = (string)objOne;
            switch (effectType)
            {
                case "Ranged Bolt":
                {
                    List<Point> path = (List<Point>)objTwo;
                    m_onDoneDelegate = (OnEffectDone)objThree;
                    Color color = (Color)objFour;
                    m_gameInstance.SendPaintersRequest(new ShowRangedBolt(OnEffectDone, path, color));
                    break;
                }
                default:
                    throw new System.ArgumentException("MapEffects with no valid effect: " + effectType);
            }
            m_gameInstance.UpdatePainters();
        }

        private void OnEffectDone()
        {
            m_onDoneDelegate();
            m_gameInstance.ResetHandlerName();
            m_gameInstance.UpdatePainters();
        }
    }
}

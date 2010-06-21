using Magecrawl.Interfaces;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class ViewmodeKeystrokeHandler : BaseKeystrokeHandler
    {
        private Point SelectionPoint { get; set; }

        public ViewmodeKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
        }

        public override void NowPrimaried(object request)
        {
            SelectionPoint = m_engine.Player.Position;
            m_gameInstance.SendPaintersRequest(new EnableMapCursor(true, SelectionPoint));
            m_gameInstance.SendPaintersRequest(new EnableToolTips(true));
            m_gameInstance.UpdatePainters();
        }

        private void ViewMode()
        {
            Escape();
        }

        private void Escape()
        {
            m_gameInstance.SendPaintersRequest(new EnableMapCursor(false));
            m_gameInstance.SendPaintersRequest(new EnableToolTips(false));
            m_gameInstance.UpdatePainters();
            m_gameInstance.ResetHandlerName();
        }

        private void HandleDirection(Direction direction)
        {
            SelectionPoint = PointDirectionUtils.ConvertDirectionToDestinationPoint(SelectionPoint, direction);
            m_gameInstance.SendPaintersRequest(new ChangeCursorPosition(SelectionPoint));
            m_gameInstance.UpdatePainters();
        }

        private void North()
        {
            HandleDirection(Direction.North);
        }

        private void South()
        {
            HandleDirection(Direction.South);
        }

        private void East()
        {
            HandleDirection(Direction.East);
        }

        private void West()
        {
            HandleDirection(Direction.West);
        }

        private void Northeast()
        {
            HandleDirection(Direction.Northeast);
        }

        private void Northwest()
        {
            HandleDirection(Direction.Northwest);
        }

        private void Southeast()
        {
            HandleDirection(Direction.Southeast);
        }

        private void Southwest()
        {
            HandleDirection(Direction.Southwest);
        }
    }
}

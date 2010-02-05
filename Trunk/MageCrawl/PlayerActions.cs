using System.Collections.Generic;
using Magecrawl.Exceptions;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs;
using Magecrawl.Keyboard;
using Magecrawl.Utilities;

namespace Magecrawl
{
    class PlayerActions
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;
        private AutoTraveler m_autoTraveler;

        public PlayerActions(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_autoTraveler = new AutoTraveler(engine, instance);
        }

        public void Move(Direction d)
        {
            m_engine.MovePlayer(d);
            m_gameInstance.UpdatePainters();
        }

        public void Run(Direction d)
        {
            m_autoTraveler.RunInDirection(d);
            m_gameInstance.UpdatePainters();
        }

        public void GetItem()
        {
            m_engine.PlayerGetItem();
            m_gameInstance.UpdatePainters();
        }

        public void Wait()
        {
            m_engine.PlayerWait();
            m_gameInstance.UpdatePainters();
        }

        public void Operate(NamedKey operateKey)
        {
            List<EffectivePoint> targetPoints = CalculateOperatePoints();
            if (Preferences.Instance.SinglePressOperate && targetPoints.Count == 1)
            {
                m_engine.Operate(targetPoints[0].Position);
                m_gameInstance.UpdatePainters();
            }
            else
            {
                OnTargetSelection operateDelegate = new OnTargetSelection(OnOperate);
                m_gameInstance.SetHandlerName("Target", targetPoints, operateDelegate, operateKey, TargettingKeystrokeHandler.TargettingType.Operatable);
            }
        }

        public void Attack(NamedKey attackKey)
        {
            if (!m_engine.Player.CurrentWeapon.IsLoaded)
            {
                m_engine.ReloadWeapon();
                m_gameInstance.TextBox.AddText(string.Format("{0} reloads the {1}.", m_engine.Player.Name, m_engine.Player.CurrentWeapon.DisplayName));
            }
            else
            {
                List<EffectivePoint> targetPoints = m_engine.Player.CurrentWeapon.CalculateTargetablePoints();
                OnTargetSelection attackDelegate = new OnTargetSelection(OnAttack);
                m_gameInstance.SetHandlerName("Target", targetPoints, attackDelegate, attackKey, TargettingKeystrokeHandler.TargettingType.Monster);
            }
        }

        public void SwapWeapon()
        {
            m_engine.PlayerSwapPrimarySecondaryWeapons();
            m_gameInstance.UpdatePainters();
        }

        public void DownStairs()
        {
            HandleStairs(m_engine.PlayerMoveDownStairs);
        }

        public void UpStairs()
        {
            HandleStairs(m_engine.PlayerMoveUpStairs);
        }

        public void MoveToLocation(NamedKey movementKey)
        {
            m_autoTraveler.MoveToLocation(movementKey);
        }

        #region HelperMethods

        private delegate bool StairMovement();
        private void HandleStairs(StairMovement s)
        {
            StairMovmentType stairMovement = m_engine.IsStairMovementSpecial(s == m_engine.PlayerMoveUpStairs);
            switch (stairMovement)
            {
                case StairMovmentType.QuitGame:
                    m_gameInstance.SetHandlerName("QuitGame", QuitReason.leaveDungeom);
                    break;
                case StairMovmentType.WinGame:
                    // Don't save if player closes window with dialog up.
                    m_gameInstance.ShouldSaveOnClose = false;
                    string winString = "Congratulations, you have completed the magecrawl tech demo! " + m_engine.Player.Name + " continues on without you in search of further treasure and fame. Consider telling your story to others, including the creator.";
                    m_gameInstance.SetHandlerName("OneButtonDialog", winString, new OnOneButtonComplete(OnWinDialogComplete));
                    break;
                case StairMovmentType.None:
                    s();
                    m_gameInstance.UpdatePainters();
                    return;
            }
        }

        private void OnWinDialogComplete()
        {
            throw new PlayerWonException();
        }

        private bool OnAttack(Point selection)
        {
            if (selection != m_engine.Player.Position)
                m_engine.PlayerAttack(selection);
            return false;
        }

        private List<EffectivePoint> CalculateOperatePoints()
        {
            List<EffectivePoint> listOfSelectablePoints = new List<EffectivePoint>();

            foreach (IMapObject mapObj in m_engine.Map.MapObjects)
            {
                if (PointDirectionUtils.LatticeDistance(m_engine.Player.Position, mapObj.Position) == 1)
                {
                    listOfSelectablePoints.Add(new EffectivePoint(mapObj.Position, 1.0f));
                }
            }

            return listOfSelectablePoints;
        }

        private bool OnOperate(Point selection)
        {
            if (selection != m_engine.Player.Position)
                m_engine.Operate(selection);
            return false;
        }

        #endregion
    }
}

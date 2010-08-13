using System.Collections.Generic;
using System.Linq;
using libtcod;
using Magecrawl.Exceptions;
using Magecrawl.GameUI.Dialogs;
using Magecrawl.Interfaces;
using Magecrawl.Keyboard;
using Magecrawl.Keyboard.Requests;
using Magecrawl.Utilities;

namespace Magecrawl
{
    internal class PlayerActions
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
            bool didAct = m_engine.Actions.Move(d);

            Point targetPosition = PointDirectionUtils.ConvertDirectionToDestinationPoint(m_engine.Player.Position, d); 
            if (!didAct && (bool)Preferences.Instance["BumpToAttack"])
            {
                if (m_engine.Map.Monsters.Where(m => m.Position == targetPosition).Count() > 0)
                    didAct = m_engine.Actions.Attack(targetPosition);
            }

            if (!didAct && (bool)Preferences.Instance["BumpToOpenDoors"])
            {
                if (m_engine.Map.MapObjects.Where(i => i.Name == "Closed Door" && i.Position == targetPosition).Count() > 0)
                    didAct = m_engine.Actions.Operate(targetPosition);
            }

            m_gameInstance.UpdatePainters();
        }

        public void Run(Direction d)
        {
            m_autoTraveler.RunInDirection(d);
            m_gameInstance.UpdatePainters();
        }

        public void GetItem()
        {
            List<INamedItem> itemsAtLocation = m_engine.Map.Items.Where(i => i.Second == m_engine.Player.Position).Select(i => i.First).OfType<INamedItem>().ToList();
            if (itemsAtLocation.Count > 1)
            {
                m_gameInstance.SetHandlerName("ItemOnGroundSelection", itemsAtLocation);
                return;
            }
            else
            {
                m_engine.Actions.GetItem();
                m_gameInstance.UpdatePainters();
            }
        }

        public void Wait()
        {
            m_engine.Actions.Wait();
            m_gameInstance.UpdatePainters();
        }

        public void RestUntilHealed()
        {
            while (m_engine.Player.CurrentStamina < m_engine.Player.MaxStamina || m_engine.Player.CurrentMP < m_engine.Player.MaxMP 
                || (m_engine.Player.IsRegenerating && m_engine.Player.CurrentHealth < m_engine.Player.MaxHealth))
            {
                if (m_engine.GameState.DangerInLOS())
                    break;
                
                // If user hits a key while resting, stop
                if (TCODConsole.checkForKeypress((int)TCODKeyStatus.KeyPressed).Pressed)
                    break;

                m_engine.Actions.Wait();
                m_gameInstance.UpdatePainters();
                m_gameInstance.DrawFrame();
            }
        }

        public void Operate(NamedKey operateKey)
        {
            List<EffectivePoint> targetPoints = CalculateOperatePoints();
            if ((bool)Preferences.Instance["SinglePressOperate"] && targetPoints.Count == 1)
            {
                m_engine.Actions.Operate(targetPoints[0].Position);
                m_gameInstance.UpdatePainters();
            }
            else
            {
                OnTargetSelection operateDelegate = new OnTargetSelection(OnOperate);
                m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetPoints, operateDelegate, operateKey, TargettingKeystrokeHandler.TargettingType.Operatable));
            }
        }

        public void Attack(NamedKey attackKey)
        {
            if (m_engine.Player.CurrentWeapon.IsRanged && !m_engine.Player.CurrentWeapon.IsLoaded)
            {
                m_engine.Actions.ReloadWeapon();
                m_gameInstance.TextBox.AddText(string.Format("{0} reloads the {1}.", m_engine.Player.Name, m_engine.Player.CurrentWeapon.DisplayName));
            }
            else
            {
                List<EffectivePoint> targetPoints = m_engine.GameState.CalculateTargetablePointsForEquippedWeapon();
                OnTargetSelection attackDelegate = new OnTargetSelection(OnRangedAttack);
                m_gameInstance.SetHandlerName("Target", new TargettingKeystrokeRequest(targetPoints, attackDelegate, attackKey, TargettingKeystrokeHandler.TargettingType.Monster));
            }
        }

        public void SwapWeapon()
        {
            m_engine.Actions.SwapPrimarySecondaryWeapons();
            m_gameInstance.UpdatePainters();
        }

        public void DownStairs()
        {
            HandleStairs(m_engine.Actions.MoveDownStairs);
        }

        public void UpStairs()
        {
            HandleStairs(m_engine.Actions.MoveUpStairs);
        }

        public void MoveToLocation(NamedKey movementKey)
        {
            m_autoTraveler.MoveToLocation(movementKey);
        }

        #region HelperMethods

        private delegate bool StairMovement();
        private void HandleStairs(StairMovement s)
        {
            StairMovmentType stairMovement = m_engine.GameState.IsStairMovementSpecial(s == m_engine.Actions.MoveUpStairs);
            switch (stairMovement)
            {
                case StairMovmentType.QuitGame:
                    m_gameInstance.SetHandlerName("QuitGame", QuitReason.leaveDungeom);
                    break;
                case StairMovmentType.WinGame:
                    // Don't save if player closes window with dialog up.
                    m_gameInstance.ShouldSaveOnClose = false;
                    string winString = "Congratulations, you have completed the magecrawl tech demo! " + m_engine.Player.Name + " continues on without you in search of further treasure and fame. Consider telling your story to others, including the creator.";
                    m_gameInstance.SetHandlerName("OneButtonDialog", new Pair<OnOneButtonComplete, string>(OnWinDialogComplete, winString));
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

        private bool OnRangedAttack(Point selection)
        {
            if (selection != m_engine.Player.Position)
                m_engine.Actions.Attack(selection);
            return false;
        }

        private List<EffectivePoint> CalculateOperatePoints()
        {
            List<EffectivePoint> listOfSelectablePoints = new List<EffectivePoint>();

            foreach (IMapObject mapObj in m_engine.Map.MapObjects)
            {
                if (PointDirectionUtils.LatticeDistance(m_engine.Player.Position, mapObj.Position) == 1)
                {
                    // If there are any monsters at that location, don't select it.
                    if (m_engine.Map.Monsters.Where(x => x.Position == mapObj.Position).Count() == 0)
                        listOfSelectablePoints.Add(new EffectivePoint(mapObj.Position, 1.0f));
                }
            }

            return listOfSelectablePoints;
        }

        private bool OnOperate(Point selection)
        {
            if (selection != m_engine.Player.Position)
                m_engine.Actions.Operate(selection);
            return false;
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using libtcodWrapper;
using Magecrawl.Exceptions;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameUI.Dialogs;
using Magecrawl.GameUI.Map.Requests;
using Magecrawl.GameUI.MapEffects;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard
{
    internal class DefaultKeystrokeHandler : BaseKeystrokeHandler
    {
        private IGameEngine m_engine;
        private GameInstance m_gameInstance;
        private AutoTraveler m_autoTraveler;

        public DefaultKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_keyMappings = null;
            m_autoTraveler = new AutoTraveler(engine, instance);
        }

        #region Mappable key commands

        /*
         * BCL: see file MageCrawl/dist/KeyMappings.xml. To add a new mappable action, define a private method for it here,
         * then map it to an unused key in KeyMappings.xml. The action should take no parameters and should return nothing.
         */

        // If you add new non-debug commands, remember to update HelpPainter.cs
        private void North()
        {
            m_engine.MovePlayer(Direction.North);
            m_gameInstance.UpdatePainters();
        }

        private void South()
        {
            m_engine.MovePlayer(Direction.South);
            m_gameInstance.UpdatePainters();
        }

        private void East()
        {
            m_engine.MovePlayer(Direction.East);
            m_gameInstance.UpdatePainters();
        }

        private void West()
        {
            m_engine.MovePlayer(Direction.West);
            m_gameInstance.UpdatePainters();
        }

        private void Northeast()
        {
            m_engine.MovePlayer(Direction.Northeast);
            m_gameInstance.UpdatePainters();
        }

        private void Northwest()
        {
            m_engine.MovePlayer(Direction.Northwest);
            m_gameInstance.UpdatePainters();
        }

        private void Southeast()
        {
            m_engine.MovePlayer(Direction.Southeast);
            m_gameInstance.UpdatePainters();
        }

        private void Southwest()
        {
            m_engine.MovePlayer(Direction.Southwest);
            m_gameInstance.UpdatePainters();
        }

        private void RunNorth()
        {
            m_autoTraveler.RunInDirection(Direction.North);
            m_gameInstance.UpdatePainters();
        }

        private void RunSouth()
        {
            m_autoTraveler.RunInDirection(Direction.South);
            m_gameInstance.UpdatePainters();
        }

        private void RunEast()
        {
            m_autoTraveler.RunInDirection(Direction.East);
            m_gameInstance.UpdatePainters();
        }

        private void RunWest()
        {
            m_autoTraveler.RunInDirection(Direction.West);
            m_gameInstance.UpdatePainters();
        }

        private void RunNortheast()
        {
            m_autoTraveler.RunInDirection(Direction.Northeast);
            m_gameInstance.UpdatePainters();
        }

        private void RunNorthwest()
        {
            m_autoTraveler.RunInDirection(Direction.Northwest);
            m_gameInstance.UpdatePainters();
        }

        private void RunSoutheast()
        {
            m_autoTraveler.RunInDirection(Direction.Southeast);
            m_gameInstance.UpdatePainters();
        }

        private void RunSouthwest()
        {
            m_autoTraveler.RunInDirection(Direction.Southwest);
            m_gameInstance.UpdatePainters();
        }

        private void Quit()
        {
            m_gameInstance.SetHandlerName("QuitGame", QuitReason.quitAction);
        }

        private void Operate()
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
                NamedKey operateKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
                m_gameInstance.SetHandlerName("Target", targetPoints, operateDelegate, operateKey, TargettingKeystrokeHandler.TargettingType.Operatable);
            }
        }

        private bool OnOperate(Point selection)
        {
            if (selection != m_engine.Player.Position)
                m_engine.Operate(selection);
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

        private void GetItem()
        {
            m_engine.PlayerGetItem();
            m_gameInstance.UpdatePainters();
        }

        private void Save()
        {
            m_gameInstance.SetHandlerName("SaveGame");
        }

        private void DebugMoveableOnOff()
        {
            if (Preferences.Instance.DebuggingMode)
            {
                m_gameInstance.SendPaintersRequest(new ToggleDebuggingMoveable(m_engine));
                m_gameInstance.UpdatePainters();
            }
        }

        private void DebuggingFOVOnOff()
        {
            if (Preferences.Instance.DebuggingMode)
            {
                m_gameInstance.SendPaintersRequest(new ToggleDebuggingFOV(m_engine));
                m_gameInstance.UpdatePainters();
            }
        }

        private void DebugFOVOnOff()
        {
            if (Preferences.Instance.DebuggingMode)
            {
                m_gameInstance.SendPaintersRequest(new SwapFOVEnabledStatus());
                m_gameInstance.UpdatePainters();
            }
        }

        private void Wait()
        {
            m_engine.PlayerWait();
            m_gameInstance.UpdatePainters();
        }

        private NamedKey GetNamedKeyForMethodInfo(MethodInfo info)
        {
            return m_keyMappings.Keys.Where(k => m_keyMappings[k] == info).Single();
        }

        private void Attack()
        {
            if (!m_engine.Player.CurrentWeapon.IsLoaded)
            {
                m_engine.ReloadWeapon();
                m_gameInstance.TextBox.AddText(string.Format("{0} reloads the {0}.", m_engine.Player.Name, m_engine.Player.CurrentWeapon.DisplayName));
            }
            else
            {
                List<EffectivePoint> targetPoints = m_engine.Player.CurrentWeapon.CalculateTargetablePoints();
                OnTargetSelection attackDelegate = new OnTargetSelection(OnAttack);
                NamedKey attackKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
                m_gameInstance.SetHandlerName("Target", targetPoints, attackDelegate, attackKey, TargettingKeystrokeHandler.TargettingType.Monster);
            }
        }

        private class SingleRangedAnimationHelper
        {
            private Point m_point;
            private IGameEngine m_engine;
            private GameInstance m_gameInstance;

            internal SingleRangedAnimationHelper(Point point, IGameEngine engine, GameInstance gameInstance)
            {
                m_point = point;
                m_engine = engine;
                m_gameInstance = gameInstance;
            }

            internal void Invoke()
            {
                m_engine.PlayerAttack(m_point);
                m_gameInstance.ResetHandlerName();
                m_gameInstance.UpdatePainters();
            }
        }

        private bool OnAttack(Point selection)
        {
            if (selection != m_engine.Player.Position)
            {
                if (m_engine.Player.CurrentWeapon.IsRanged)
                {
                    List<Point> pathToTarget = m_engine.PlayerPathToPoint(selection);
                    SingleRangedAnimationHelper rangedHelper = new SingleRangedAnimationHelper(selection, m_engine, m_gameInstance);
                    EffectDone onEffectDone = new EffectDone(rangedHelper.Invoke);
                    Color color = TCODColorPresets.White;
                    m_gameInstance.SetHandlerName("Effects", new ShowRangedBolt(onEffectDone, pathToTarget, color));
                    return true;
                }
                else
                {
                    m_engine.PlayerAttack(selection);
                    return false;
                }
            }
            return false;
        }

        private void ViewMode()
        {
            m_gameInstance.SetHandlerName("Viewmode");
        }

        private void Inventory()
        {
            m_gameInstance.SetHandlerName("Inventory");
        }

        private void Equipment()
        {
            m_gameInstance.SetHandlerName("Equipment");
        }

        private void SwapWeapon()
        {
            m_engine.PlayerSwapPrimarySecondaryWeapons();
            m_gameInstance.UpdatePainters();
        }

        private void TextBoxPageUp()
        {
            m_gameInstance.TextBox.TextBoxScrollUp();
        }

        private void TextBoxPageDown()
        {
            m_gameInstance.TextBox.TextBoxScrollDown();
        }

        private void TextBoxClear()
        {
            m_gameInstance.TextBox.Clear();
        }

        private void CastSpell()
        {
            NamedKey castKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_gameInstance.SetHandlerName("SpellList", castKey);
        }
        
        private void Escape()
        {
        }

        private void Select()
        {
        }

        private void Help()
        {
            m_gameInstance.SetHandlerName("Help", m_keyMappings);
        }

        private void DownStairs()
        {
            HandleStairs(m_engine.PlayerMoveDownStairs);
        }

        private void UpStairs()
        {
            HandleStairs(m_engine.PlayerMoveUpStairs);
        }

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

        private void MoveToLocation()
        {
            NamedKey movementKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_autoTraveler.MoveToLocation(movementKey);
        }

        // If you add new non-debug commands, remember to update HelpPainter.cs
        #endregion
    }
}

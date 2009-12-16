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

        public DefaultKeystrokeHandler(IGameEngine engine, GameInstance instance)
        {
            m_engine = engine;
            m_gameInstance = instance;
            m_keyMappings = null;
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
            List<EffectivePoint> targetPoints = m_engine.Player.CurrentWeapon.CalculateTargetablePoints();
            OnTargetSelection attackDelegate = new OnTargetSelection(OnAttack);
            NamedKey attackKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_gameInstance.SetHandlerName("Target", targetPoints, attackDelegate, attackKey, TargettingKeystrokeHandler.TargettingType.Monster);
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
            List<EffectivePoint> targetPoints = GeneratePointsOneCanMoveTo();
            OnTargetSelection movementDelegate = new OnTargetSelection(OnMovementLocationSelected);
            NamedKey movementKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_gameInstance.SetHandlerName("Target", targetPoints, movementDelegate, movementKey, TargettingKeystrokeHandler.TargettingType.OpenFloor);
        }

        private bool OnMovementLocationSelected(Point selected)
        {
            // Don't show the overlap as we travel
            m_gameInstance.SendPaintersRequest(new EnableMapCursor(false));
            m_gameInstance.SendPaintersRequest(new EnablePlayerTargeting(false));

            while (!m_engine.DangerInLOS())
            {
                List<Point> pathToPoint = m_engine.PlayerPathToPoint(selected);
                if (pathToPoint.Count == 0)
                    return false;

                Direction d = PointDirectionUtils.ConvertTwoPointsToDirection(m_engine.Player.Position, pathToPoint[0]);
                
                m_engine.MovePlayer(d);
                m_gameInstance.UpdatePainters();

                m_gameInstance.DrawFrame();
            }
            return false;
        }

        private List<EffectivePoint> GeneratePointsOneCanMoveTo()
        {
            List<EffectivePoint> returnList = new List<EffectivePoint>();
            for (int i = 0; i < m_engine.Map.Width; ++i)
            {
                for (int j = 0; j < m_engine.Map.Height; ++j)
                {
                    Point p = new Point(i,j);
                    // We can move there is we've visited it is a floor and there is no solid object there.
                    if (m_engine.Map.GetTerrainAt(p) == TerrainType.Floor && m_engine.Map.IsVisitedAt(p) && m_engine.Map.MapObjects.Where(x => x.Position == p && x.IsSolid).Count() == 0)
                    {
                        returnList.Add(new EffectivePoint(p, 1));
                    }
                }
            }

            return returnList;
        }


        // If you add new non-debug commands, remember to update HelpPainter.cs
        #endregion
    }
}

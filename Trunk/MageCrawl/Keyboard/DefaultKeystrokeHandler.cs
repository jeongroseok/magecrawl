using System.Collections.Generic;
using System.Reflection;
using libtcodWrapper;
using Magecrawl.GameEngine.Interfaces;
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
            m_gameInstance.SetHandlerName("QuitGame");
        }

        private void Operate()
        {
            List<EffectivePoint> targetPoints = CalculateOperatePoints();
            OnTargetSelection operateDelegate = new OnTargetSelection(OnOperate);
            NamedKey operateKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            if (targetPoints.Count == 1)
                m_gameInstance.SetHandlerName("Target", targetPoints, operateDelegate, operateKey, targetPoints[0].Position);
            else
                m_gameInstance.SetHandlerName("Target", targetPoints, operateDelegate, operateKey);
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

        private void Load()
        {
            try
            {
                m_engine.Load();
                m_gameInstance.UpdatePainters();
            }
            catch (System.IO.FileNotFoundException)
            {
                // TODO: Inform user somehow
                m_gameInstance.UpdatePainters();
            }
        }

        private void MoveableOnOff()
        {
            m_gameInstance.SendPaintersRequest(new ToggleDebuggingMoveable(m_engine));
            m_gameInstance.UpdatePainters();
        }

        private void DebuggingFOVOnOff()
        {
            m_gameInstance.SendPaintersRequest(new ToggleDebuggingFOV(m_engine));
            m_gameInstance.UpdatePainters();
        }

        private void FOVOnOff()
        {
            m_gameInstance.SendPaintersRequest(new SwapFOVEnabledStatus());
            m_gameInstance.UpdatePainters();
        }

        private void Wait()
        {
            m_engine.PlayerWait();
            m_gameInstance.UpdatePainters();
        }

        private NamedKey GetNamedKeyForMethodInfo(MethodInfo info)
        {
            foreach (NamedKey key in m_keyMappings.Keys)
            {
                if (m_keyMappings[key] == info)
                    return key;
            }
            throw new System.ArgumentException("GetNamedKeyForMethodInfo - Can't find NamedKey for method?");
        }

        private void Attack()
        {
            List<EffectivePoint> targetPoints = m_engine.Player.CurrentWeapon.CalculateTargetablePoints();
            OnTargetSelection attackDelegate = new OnTargetSelection(OnAttack);
            NamedKey attackKey = GetNamedKeyForMethodInfo((MethodInfo)MethodInfo.GetCurrentMethod());
            m_gameInstance.SetHandlerName("Target", targetPoints, attackDelegate, attackKey);
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

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine
{
    internal sealed class CoreTimingEngine
    {
        private const int CTNeededForNewTurn = 100;
        private const int CTPerIteration = 5;
        private const int CTBaseCostToMove = 100;
        private const int CTBaseCostForAction = 100;

        internal CoreTimingEngine()
        {
        }

        internal Character GetNextActor(CoreGameEngine engine)
        {
            List<Character> actors = new List<Character>();
            actors.Add(engine.Player as Player);

            // Until we find a winner
            while (true)
            {
                // Check all valid actors for ability to go now
                foreach (Character currentActor in actors)
                {
                    if (currentActor.CT >= CTNeededForNewTurn)
                        return currentActor;
                }

                // No actors can go now, so incremenet each one's CT
                foreach (Character currentActor in actors)
                {
                    currentActor.CT += CTPerIteration;
                }
            }
        }

        internal void ActorMadeMove(Character c)
        {
            c.CT -= (int)(CTBaseCostToMove * c.CTCostModifierToMove);
        }

        internal void ActorDidAction(Character c)
        {
            c.CT -= (int)(CTBaseCostForAction * c.CTCostModifierToAct);
        }
    }
}

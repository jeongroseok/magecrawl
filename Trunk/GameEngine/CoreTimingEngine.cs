using System;
using System.Collections.Generic;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;

namespace Magecrawl.GameEngine
{
    // The time system is based upon 'Charge Time', an idea borrowed from Final Fantasy Tactics and extended.
    // Each actor (player or monsters) has a CT. Every action reduces it by an amount, possibly reduced or
    // increased by their speed. When a actor has acted, we search for the next one who has at least CTNeededForNewTurn
    // If no-one has that, we interativly increase by CTPerIteration factoring in their CTIncreaseModifier until someone can act.
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
            foreach (Monster m in engine.Map.Monsters)
            {
                actors.Add(m);
            }

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
                    currentActor.CT += (int)(CTPerIteration * currentActor.CTIncreaseModifier);
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

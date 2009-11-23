using System;
using System.Collections.Generic;
using System.Linq;
using Magecrawl.GameEngine;
using Magecrawl.GameEngine.Actors;
using Magecrawl.GameEngine.Interfaces;
using Magecrawl.GameEngine.Level;

namespace Magecrawl.GameEngine
{
    // The time system is based upon 'Charge Time', an idea borrowed from Final Fantasy Tactics and extended.
    // Each actor (player or monsters) has a CT. Every action reduces it by an amount, possibly reduced or
    // increased by their speed. When a actor has acted, we search for the next one who has at least CTNeededForNewTurn
    // If no-one has that, we interativly increase by CTPerIteration factoring in their CTIncreaseModifier until someone can act.
    internal sealed class CoreTimingEngine
    {
        public const int CTNeededForNewTurn = 100;
        private const int CTPerIteration = 5;
        private const int CTBaseCostToMove = 100;
        private const int CTBaseCostForAction = 100;

        internal CoreTimingEngine()
        {
        }

        internal Character GetNextActor(Player player, Map map)
        {
            List<Character> actors = new List<Character>();
            actors.Add(player as Player);
            foreach (Monster m in map.Monsters)
            {
                actors.Add(m);
            }

            // Until we find a winner
            while (true)
            {
                // Check all valid actors for ability to go now
                Character actorWhoCanGo = actors.FirstOrDefault(a => a.CT >= CTNeededForNewTurn);
                if (actorWhoCanGo != null)
                    return actorWhoCanGo;

                // No actors can go now, so incremenet each one's CT
                actors.ForEach(a => a.IncreaseCT((int)(CTPerIteration * a.CTIncreaseModifier)));
            }
        }

        internal void ActorMadeMove(Character c)
        {
            c.DecreaseCT((int)(CTBaseCostToMove * c.CTCostModifierToMove));
        }

        internal void ActorDidAction(Character c)
        {
            c.DecreaseCT((int)(CTBaseCostForAction * c.CTCostModifierToAct));
        }
    }
}

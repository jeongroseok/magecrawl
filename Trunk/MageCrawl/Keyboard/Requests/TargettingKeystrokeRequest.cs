using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard.Requests
{
    internal class TargettingKeystrokeRequest
    {
        public TargettingKeystrokeRequest(List<EffectivePoint> targetablePoints, OnTargetSelection selectionDelegate, NamedKey alternateSelectionKey, TargettingKeystrokeHandler.TargettingType targettingType)
        {
            TargetablePoints = targetablePoints;
            TargettingType = targettingType;
            SelectionDelegate = selectionDelegate;
            AlternateSelectionKey = alternateSelectionKey;
        }

        public List<EffectivePoint> TargetablePoints;
        public OnTargetSelection SelectionDelegate;
        public NamedKey AlternateSelectionKey;
        public TargettingKeystrokeHandler.TargettingType TargettingType; 
    }
}

using System.Collections.Generic;
using Magecrawl.Utilities;

namespace Magecrawl.Keyboard.Requests
{
    public delegate List<Point> PlayerTargettingHaloDelegate(Point p);

    internal class TargettingKeystrokeRequest
    {
        public TargettingKeystrokeRequest(List<EffectivePoint> targetablePoints, OnTargetSelection selectionDelegate, NamedKey alternateSelectionKey, TargettingKeystrokeHandler.TargettingType targettingType)
            : this(targetablePoints, selectionDelegate, alternateSelectionKey, targettingType, null)
        {
        }

        public TargettingKeystrokeRequest(List<EffectivePoint> targetablePoints, OnTargetSelection selectionDelegate, NamedKey alternateSelectionKey, TargettingKeystrokeHandler.TargettingType targettingType, PlayerTargettingHaloDelegate haloDelegate)
        {
            TargetablePoints = targetablePoints;
            TargettingType = targettingType;
            SelectionDelegate = selectionDelegate;
            AlternateSelectionKey = alternateSelectionKey;
            HaloDelegate = haloDelegate;
        }

        public List<EffectivePoint> TargetablePoints;
        public OnTargetSelection SelectionDelegate;
        public NamedKey AlternateSelectionKey;
        public TargettingKeystrokeHandler.TargettingType TargettingType; 
        public PlayerTargettingHaloDelegate HaloDelegate;
    }
}

﻿using PT.PM.Common;

namespace PT.PM.Patterns.Nodes
{
    public interface IRelativeLocationMatching
    {
        TextSpan[] MatchedLocations { get; set; }
    }
}

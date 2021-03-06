﻿using System;
using System.Collections.Generic;
using PT.PM.Common;
using PT.PM.Patterns;

namespace PT.PM.Matching
{
    public interface IUstPatternMatcher<TInputGraph, TPattern, TMatchingResult> : ILoggable
        where TPattern : PatternBase
        where TMatchingResult : MatchingResultBase<TPattern>
    {
        bool IsIgnoreFilenameWildcards { get; set; }

        TPattern[] Patterns { get; set; }

        List<TMatchingResult> Match(TInputGraph ust);
    }
}

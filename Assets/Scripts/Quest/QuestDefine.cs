using System;

namespace Ninez.Quest
{
    public enum MatchType
    {
        NONE        = 0,
        THREE       = 3,    // 3 Match
        FOUR        = 4,    // 4 Match     -> CLEAR_HORZ 또는 VERT 퀘스트
        FIVE        = 5,    // 5 Match     -> CLEAR_LAZER 퀘스트
        THREE_THREE = 6,    // 3 + 3 Match -> CLEAR_CIRCLE 퀘스트 
        THREE_FOUR  = 7,    // 3 + 4 Match -> CLEAR_CIRCLE 퀘스트
        THREE_FIVE  = 8,    // 3 + 5 Match -> CLEAR_LAZER 퀘스트
        FOUR_FIVE   = 9,    // 4 + 5 Match -> CLEAR_LAZER 퀘스트
        FOUR_FOUR   = 10,   // 4 + 4 Match -> CLEAR_CIRCLE 퀘스트
    }

    static class MatchTypeMethod
    {
        public static short ToValue(this MatchType matchType)
        {
            return (short)matchType;
        }

        /*
         * 블럭의 매칭 결과를 조합한다
         */
        public static MatchType Add(this MatchType matchTypeSrc, MatchType matchTypeTarget)
        {
            if (matchTypeSrc == MatchType.FOUR && matchTypeTarget == MatchType.FOUR)
                return MatchType.FOUR_FOUR;

            return (MatchType)((int)matchTypeSrc + (int)matchTypeTarget);
        }
    }
}
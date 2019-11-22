using System.Collections.Generic;

namespace Ninez.Util
{
    public static class SortedListMethods
    {
        /**
         * SortedList 확장 메소드.
         * 첫번째 노드의 key-value 를 구한다.
         * (주의) 비어있는 경우 T1, T2 타입의 디폴값이 전달되므로 호출전에 비어있는지 체크하는 것이 안정한다.
         * 
         * 사용 예) KeyValuePair<int, Vector2) kv = sortedList.First();
         */
        public static KeyValuePair<T1, T2> First<T1, T2>(this SortedList<T1, T2> sortedList)
        {
            if (sortedList.Count == 0)
                return new KeyValuePair<T1, T2>();

            return new KeyValuePair<T1, T2>(sortedList.Keys[0], sortedList.Values[0]);
        }
    }
}
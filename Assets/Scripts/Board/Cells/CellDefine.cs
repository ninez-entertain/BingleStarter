using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Board
{
    public enum CellType
    {
        EMPTY = 0,      //빈공간, 블럭이 위치할 수 없음, 드롭 통과
        BASIC = 1,      //배경있는 기본 형 (No action)
        FIXTURE = 2,    //고정된 장애물. 변화없음
        JELLY = 3,      //젤리 : 블럭 이동 OK, 블럭 CLEAR되면 BASIC, 출력 : CellBg
    }


    //-------------------------------------------------------------------------
    // Extension Method 
    //-------------------------------------------------------------------------

    /// <summary>
    /// CellType enum extention  
    /// </summary>
    static class CellTypeMethod
    {
        /*
         * 블럭이 위치할 수 있는 타입인지 체크한다. 현재 위치한 블럭의 상태와 관계업음.
         */
        public static bool IsBlockAllocatableType(this CellType cellType)
        {
            return !(cellType == CellType.EMPTY);
        }

        /*
         * 블럭이 다른 위치로 이동 가능한 타입인지 체크한다. 현재 포함하고 있는 상태와 관계업음.
         */
        public static bool IsBlockMovableType(this CellType cellType)
        {
            return !(cellType == CellType.EMPTY);
        }
    }
}
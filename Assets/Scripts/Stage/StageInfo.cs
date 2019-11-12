using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Board;

namespace Ninez.Stage
{
    /// <summary>
    /// stage 파일에서 읽어온 정보 (스테이지 구성에 사용) 
    /// Note : 멤버 네이밍은 m_ 붙이지 않는다.
    /// JSON 파일을 serialize하는 경우 json 키 값과 변수 이름이 일치해야 된다.
    /// </summary>
    [System.Serializable]
    public class StageInfo
    {
        public int row;
        public int col;

        public int[] cells;

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// 지정된 위치의 Cell Type을 구한다
        /// </summary>
        /// <param name="nRow">행</param>
        /// <param name="nCol">열</param>
        /// <returns></returns>
        public CellType GetCellType(int nRow, int nCol)
        {
            Debug.Assert(cells != null && cells.Length > nRow * col + nCol, $"Invalid Row/Col = {nRow}, {nCol}");

            //if (cells.Length > nRow * col + nCol)
            //    return (CellType)cells[nRow * col + nCol];

            int revisedRow = (row - 1) - nRow;
            if (cells.Length > revisedRow * col + nCol)
                return (CellType)cells[revisedRow * col + nCol];

            Debug.Assert(false);

            return CellType.EMPTY;
        }

        /// <summary>
        /// 생성된 정보가 유효한지 검사한다 
        /// </summary>
        /// <returns>유효하면 ture, 그렇지 않으면 false</returns>
        public bool DoValidation()
        {
            Debug.Assert(cells.Length == row * col);
            Debug.Log($"cell length : {cells.Length}, row, col = ({row}, {col})");

            if (cells.Length != row * col)
                return false;

            return true;
        }
    }
}

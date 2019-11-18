using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Board;
using Ninez.Util;
using Ninez.Core;

namespace Ninez.Stage
{
    public class Stage
    {
        public int maxRow { get { return m_Board.maxRow; } }
        public int maxCol { get { return m_Board.maxCol; } }

        Ninez.Board.Board m_Board;
        public Ninez.Board.Board board { get { return m_Board; } }

        StageBuilder m_StageBuilder;

        public Block[,] blocks { get { return m_Board.blocks; } }
        public Cell[,] cells { get { return m_Board.cells; } }

        /// <summary>
        /// 생성자.
        /// 주어진 크기를 갖는 Board를 생성한다.
        /// </summary>
        /// <param name="stageBuilder"></param>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        public Stage(StageBuilder stageBuilder, int nRow, int nCol)
        {
            m_StageBuilder = stageBuilder;

            m_Board = new Ninez.Board.Board(nRow, nCol);
        }

        /// <summary>
        /// 주어진 정보(Cell/Block Prefab, 컨테이너)를 이용해서 Board를 구성한다.
        /// </summary>
        /// <param name="cellPrefab">Cell Prefab</param>
        /// <param name="blockPrefab">Board Prefab</param>
        /// <param name="container">Cell/Board GameObject의 부모 GameObject</param>
        internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container)
        {
            m_Board.ComposeStage(cellPrefab, blockPrefab, container);
        }

        /*
         * 유효한 스와이프인 경우 : 스와이프 방향으로 블럭 위치가 교체된다.
         * 유효지 않는 스와이프 경우 : 화면에 스와이프 방향으로 액션을 보여주고, 원상태로 복구하는 액션 보여준다
         */
        public IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir, Returnable<bool> actionResult)
        {
            actionResult.value = false; //코루틴 리턴값 RESET

            //1. 스와이프되는 상대 블럭 위치를 구한다. (using SwipeDir Extension Method)
            int nSwipeRow = nRow, nSwipeCol = nCol;
            nSwipeRow += swipeDir.GetTargetRow(); //Right : +1, LEFT : -1
            nSwipeCol += swipeDir.GetTargetCol(); //UP : +1, DOWN : -1

            Debug.Assert(nRow != nSwipeRow || nCol != nSwipeCol, "Invalid Swipe : ({nSwipeRow}, {nSwipeCol})");
            Debug.Assert(nSwipeRow >= 0 && nSwipeRow < maxRow && nSwipeCol >= 0 && nSwipeCol < maxCol, $"Swipe 타겟 블럭 인덱스 오류 = ({nSwipeRow}, {nSwipeCol}) ");

            //2. 스와이프 가능한 블럭인지 체크한다. (인덱스 Validation은 호출 전에 검증됨)
            if (m_Board.IsSwipeable(nSwipeRow, nSwipeCol))
            {
                //2.1 스와이프 대상 블럭(소스, 타겟)과 각 블럭의 이동전 위치를 저장한다.
                Block targetBlock = blocks[nSwipeRow, nSwipeCol];
                Block baseBlock = blocks[nRow, nCol];
                Debug.Assert(baseBlock != null && targetBlock != null);

                Vector3 basePos = baseBlock.blockObj.transform.position;
                Vector3 targetPos = targetBlock.blockObj.transform.position;

                //2.2 스와이프 액션을 실행한다.
                if (targetBlock.IsSwipeable(baseBlock))
                {
                    //2.2.1 상대방의 블럭 위치로 이동하는 애니메이션을 수행한다
                    baseBlock.MoveTo(targetPos, Constants.SWIPE_DURATION);
                    targetBlock.MoveTo(basePos, Constants.SWIPE_DURATION);

                    yield return new WaitForSeconds(Constants.SWIPE_DURATION);

                    //2.2.2 Board에 저장된 블럭의 위치를 교환한다
                    blocks[nRow, nCol] = targetBlock;
                    blocks[nSwipeRow, nSwipeCol] = baseBlock;

                    actionResult.value = true;
                }
            }

            yield break;
        }

        #region Simple Methods
        //----------------------------------------------------------------------
        // 조회(get/set/is) 메소드
        //----------------------------------------------------------------------

        /*
         * 보드안에서 발생한 이벤트인지 체크한다       
         */
        public bool IsInsideBoard(Vector2 ptOrg)
        {
            // 계산의 편의를 위해서 (0, 0)을 기준으로 좌표를 이동한다. 
            // 8 x 8 보드인 경우: x(-4 ~ +4), y(-4 ~ +4) -> x(0 ~ +8), y(0 ~ +8) 
            Vector2 point = new Vector2(ptOrg.x + (maxCol / 2.0f), ptOrg.y + (maxRow / 2.0f));

            if (point.y < 0 || point.x < 0 || point.y > maxRow || point.x > maxCol)
                return false;

            return true;
        }

        /*
         * 유효한 블럭(이동가능한 블럭) 위에서 있는지 체크한다.
         * @param point Wordl 좌표, 컨테이너 기준
         * @param blockPos out 파라미터, 보드에 저장된 블럭의 인덱스
         * 
         * @return 스와이프 가능하면 true
         */
        public bool IsOnValideBlock(Vector2 point, out BlockPos blockPos)
        {
            //1. World 좌표 -> 보드의 블럭 인덱스로 변환한다.
            Vector2 pos = new Vector2(point.x + (maxCol/ 2.0f), point.y + (maxRow / 2.0f));
            int nRow = (int)pos.y;
            int nCol = (int)pos.x;

            //리턴할 블럭 인덱스 생성
            blockPos = new BlockPos(nRow, nCol);

            //2. 스와이프 가능한지 체크한다.
            return board.IsSwipeable(nRow, nCol);
        }


        /**
         * 주어진 위치(nRow, nCol)에서 발생된 스와이프 액션이 유효한지 체크한다.
         */
        public bool IsValideSwipe(int nRow, int nCol, Swipe swipeDir)
        {
            switch (swipeDir)
            {
                case Swipe.DOWN: return nRow > 0; ;
                case Swipe.UP: return nRow < maxRow - 1;
                case Swipe.LEFT: return nCol > 0;
                case Swipe.RIGHT: return nCol < maxCol - 1;
                default:
                    return false;
            }
        }
        #endregion

        public void PrintAll()
        {
            System.Text.StringBuilder strCells = new System.Text.StringBuilder();
            System.Text.StringBuilder strBlocks = new System.Text.StringBuilder();

            for (int nRow = maxRow -1; nRow >=0; nRow--)
            {
                for (int nCol = 0; nCol < maxCol; nCol++)
                {
                    strCells.Append($"{cells[nRow, nCol].type}, ");
                    strBlocks.Append($"{blocks[nRow, nCol].breed}, ");
                }

                strCells.Append("\n");
                strBlocks.Append("\n");
            }

            Debug.Log(strCells.ToString());
            Debug.Log(strBlocks.ToString());
        }
    }
}
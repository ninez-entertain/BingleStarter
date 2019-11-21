using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Quest;
using Ninez.Util;

namespace Ninez.Board
{
    public class Board
    {
        int m_nRow;
        int m_nCol;

        public int maxRow { get { return m_nRow; } }
        public int maxCol { get { return m_nCol; } }

        Cell[,] m_Cells;
        public Cell[,] cells { get { return m_Cells; } }

        Block[,] m_Blocks;
        public Block[,] blocks { get { return m_Blocks; } }

        Transform m_Container;
        GameObject m_CellPrefab;
        GameObject m_BlockPrefab;

        BoardEnumerator m_Enumerator;

        public Board(int nRow, int nCol)
        {
            m_nRow = nRow;
            m_nCol = nCol;

            m_Cells = new Cell[nRow, nCol];
            m_Blocks = new Block[nRow, nCol];

            m_Enumerator = new BoardEnumerator(this);
        }

        internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container)
        {
            //1. 스테이지 구성에 필요한 Cell,Block, Container(Board) 정보를 저장한다. 
            m_CellPrefab = cellPrefab;
            m_BlockPrefab = blockPrefab;
            m_Container = container;

            //2. 3매치된 블럭이 없도록 섞는다.  
            BoardShuffler shuffler = new BoardShuffler(this, true);
            shuffler.Shuffle();

            //3. Cell, Block Prefab을 이용해서 Board에 Cell/Block GameObject를 추가한다. 
            float initX = CalcInitX(0.5f);
            float initY = CalcInitY(0.5f);
            for (int nRow = 0; nRow < m_nRow; nRow++)
                for (int nCol = 0; nCol < m_nCol; nCol++)
                {
                    //3.1 Cell GameObject 생성을 요청한다.GameObject가 생성되지 않는 경우에 null을 리턴한다.
                    Cell cell = m_Cells[nRow, nCol]?.InstantiateCellObj(cellPrefab, container);
                    cell?.Move(initX + nCol, initY + nRow);

                    //3.2 Block GameObject 생성을 요청한다.
                    //    GameObject가 생성되지 않는 경우에 null을 리턴한다. EMPTY 인 경우에 null
                    Block block = m_Blocks[nRow, nCol]?.InstantiateBlockObj(blockPrefab, container);
                    block?.Move(initX + nCol, initY + nRow);
                }
        }

        /**
         * 호출 결과 : 3 매칭된 블럭이 제거된다.
         */
        public IEnumerator Evaluate(Returnable<bool> matchResult)
        {
            //1. 모든 블럭의 매칭 정보(개수, 상태, 내구도)를 계산한 후, 3매치 블럭이 있으면 true 리턴 
            bool bMatchedBlockFound = UpdateAllBlocksMatchedStatus();

            //2. 3매칭 블럭 없는 경우 
            if(bMatchedBlockFound == false)
            {
                matchResult.value = false;
                yield break;
            }

            //3. 3매칭 블럭 있는 경우

            //3.1. 첫번째 phase
            //   매치된 블럭에 지정된 액션을 수행한.
            //   ex) 가로줄의 블럭 전체가 클리어 되는 블럭인 경우에 처리 등
            for (int nRow = 0; nRow < m_nRow; nRow++)
                for (int nCol = 0; nCol < m_nCol; nCol++)
                {
                    Block block = m_Blocks[nRow, nCol];

                    block?.DoEvaluation(m_Enumerator, nRow, nCol);
                }
                
            //3.2. 두번째 phase
            //   첫번째 Phase에서 반영된 블럭의 상태값에 따라서 블럭의 최종 상태를 반영한.
            List<Block> clearBlocks = new List<Block>();

            for (int nRow = 0; nRow < m_nRow; nRow++)
            {
                for (int nCol = 0; nCol < m_nCol; nCol++)
                {
                    Block block = m_Blocks[nRow, nCol];

                    if (block != null)
                    {
                        if (block.status == BlockStatus.CLEAR)
                        {
                            clearBlocks.Add(block);

                            m_Blocks[nRow, nCol] = null;    //보드에서 블럭 제거 (블럭 객체 제거 X)
                        }
                    }
                }
            }

            //3.3 매칭된 블럭을 제거한다. 
            clearBlocks.ForEach((block) => block.Destroy());

            //3.4 3매칭 블럭 있는 경우 true 설정   
            matchResult.value = true;

            yield break;
        }

        /*
         * 모든 블럭의 상태를 현재 블럭 구성 정보를 기준으로 업데이트 한다. (주로 회전 이후 블럭의 각 상태를 업데이트하기 위해 호출된다)
         * ex) 3개이상 매치된 블럭은 매치상태 설정 등
         */
        public bool UpdateAllBlocksMatchedStatus()
        {
            List<Block> matchedBlockList = new List<Block>();
            int nCount = 0;
            for (int nRow = 0; nRow < m_nRow; nRow++)
            {
                for (int nCol = 0; nCol < m_nCol; nCol++)
                {
                    if (EvalBlocksIfMatched(nRow, nCol, matchedBlockList))
                    {
                        nCount++;
                    }
                }
            }

            return nCount > 0;
        }

        /*
         * 지정된 row, col의 블럭이 Match 블럭인지 판단한다.
         * @param matchedBlockList GC 발생을 제거하기 위해 Caller에서 생성해서 전달받는다    
         */
        public bool EvalBlocksIfMatched(int nRow, int nCol, List<Block> matchedBlockList)
        {
            bool bFound = false;

            Block baseBlock = m_Blocks[nRow, nCol];
            if (baseBlock == null)
                return false;

            if (baseBlock.match != Ninez.Quest.MatchType.NONE || !baseBlock.IsValidate() || m_Cells[nRow, nCol].IsObstracle())
                return false;

            //검사하는 자신을 매칭 리스트에 우선 보관한다.
            matchedBlockList.Add(baseBlock);

            //1. 가로 블럭 검색
            Block block;

            //1.1 오른쪽 방향
            for (int i = nCol + 1; i < m_nCol; i++)
            {
                block = m_Blocks[nRow, i];
                if (!block.IsSafeEqual(baseBlock))
                    break;

                matchedBlockList.Add(block);
            }

            //1.2 왼쪽 방향
            for (int i = nCol - 1; i >= 0; i--)
            {
                block = m_Blocks[nRow, i];
                if (!block.IsSafeEqual(baseBlock))
                    break;

                matchedBlockList.Insert(0, block);
            }

            //1.3 매치된 상태인지 판단한다
            //    기준 블럭(baseBlock)을 제외하고 좌우에 2개이상이면 기준블럭 포함해서 3개이상 매치되는 경우로 판단할 수 있다
            if (matchedBlockList.Count >= 3)
            {
                SetBlockStatusMatched(matchedBlockList, true);
                bFound = true;
            }

            matchedBlockList.Clear();

            //2. 세로 블럭 검색
            matchedBlockList.Add(baseBlock);

            //2.1 위쪽 검색
            for (int i = nRow + 1; i < m_nRow; i++)
            {
                block = m_Blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                    break;

                matchedBlockList.Add(block);
            }

            //2.2 아래쪽 검색
            for (int i = nRow - 1; i >= 0; i--)
            {
                block = m_Blocks[i, nCol];
                if (!block.IsSafeEqual(baseBlock))
                    break;

                matchedBlockList.Insert(0, block);
            }

            //2.3 매치된 상태인지 판단한다
            //    기준 블럭(baseBlock)을 제외하고 상하에 2개이상이면 기준블럭 포함해서 3개이상 매치되는 경우로 판단할 수 있다
            if (matchedBlockList.Count >= 3)
            {
                SetBlockStatusMatched(matchedBlockList, false);
                bFound = true;
            }

            //계산위해 리스트에 저장한 블럭 제거
            matchedBlockList.Clear();

            return bFound;
        }

        /*
         * 리스트에 포함된 전체 블럭의 상태를 MATCH로 변경한다.
         * @param bHorz 매치된 방향 true이면 세로방향, false이면 가로방향    
         */
        void SetBlockStatusMatched(List<Block> blockList, bool bHorz)
        {
            int nMatchCount = blockList.Count;
            blockList.ForEach(block => block.UpdateBlockStatusMatched((MatchType)nMatchCount));
        }

        /// <summary>
        /// 퍼즐의 시작 X 위치를 구한다, left - top좌표
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public float CalcInitX(float offset = 0)
        {
            return -m_nCol / 2.0f + offset;   
        }

        //퍼즐의 시작 Y 위치, left - bottom 좌표
        //하단이 (0, 0) 이므로, 
        public float CalcInitY(float offset = 0)
        {
            return -m_nRow / 2.0f + offset;
        }

        /*
         * 지정된 위치가 셔플 가능한 조건인지 체크한다
         * @bLoading true if stage being loading , on playing is false
         */
        public bool CanShuffle(int nRow, int nCol, bool bLoading)
        {
            if (!m_Cells[nRow, nCol].type.IsBlockMovableType())
                return false;

            return true;
        }

        /*
         * Block의 종류(breed)를 변경한다.
         */
        public void ChangeBlock(Block block, BlockBreed notAllowedBreed)
        {
            BlockBreed genBreed;

            while (true)
            {
                genBreed = (BlockBreed)UnityEngine.Random.Range(0, 6); //TODO 스테이지파일에서 Spawn 정책을 이용해야함

                if (notAllowedBreed == genBreed)
                    continue;

                break;
            }

            block.breed = genBreed;
        }

        public bool IsSwipeable(int nRow, int nCol)
        {
            return m_Cells[nRow, nCol].type.IsBlockMovableType();
        }
    }
}
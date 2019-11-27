using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Quest;
using Ninez.Util;
using Ninez.Stage;

namespace Ninez.Board
{
    using IntIntKV = KeyValuePair<int, int>;

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
        StageBuilder m_StageBuilder;

        BoardEnumerator m_Enumerator;

        public Board(int nRow, int nCol)
        {
            m_nRow = nRow;
            m_nCol = nCol;

            m_Cells = new Cell[nRow, nCol];
            m_Blocks = new Block[nRow, nCol];

            m_Enumerator = new BoardEnumerator(this);
        }

        internal void ComposeStage(GameObject cellPrefab, GameObject blockPrefab, Transform container, StageBuilder stageBuilder)
        {
            //1. 스테이지 구성에 필요한 Cell,Block, Container(Board) 정보를 저장한다. 
            m_CellPrefab = cellPrefab;
            m_BlockPrefab = blockPrefab;
            m_Container = container;
            m_StageBuilder = stageBuilder;

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
            //3.3.1 블럭이 제거되는 동안 잠시 Delay, 블럭 제거가 순식간에 일어나는 것에 약간 지연을 시킴
            yield return new WaitForSeconds(0.2f);

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

        /*
         * 전체 블럭 구성을 재배치한다.
         * 비어있는 블럭을 위에 있는 블럭으로 채운다.
         * - MATCH 블럭이 제거된 후에 호출된다.
         * 
         * @param unfilledBlocks 다른 블럭으로 채워지지 않고 남겨진 블럭 위치를 리턴받기 위해서 Caller에서 전달한다.  
         */
        public IEnumerator ArrangeBlocksAfterClean(List<IntIntKV> unfilledBlocks, List<Block> movingBlocks)
        {
            SortedList<int, int> emptyBlocks = new SortedList<int, int>();
            List<IntIntKV> emptyRemainBlocks = new List<IntIntKV>();

            for (int nCol = 0; nCol < m_nCol; nCol++)
            {
                emptyBlocks.Clear();

                //1.같은 열(col)에 빈 블럭을 수집한다.
                //  현재 col의 다른 row의 비어있는 븝럭 인덱스를 수집한다. sortedList이므로 첫번째 노드가 가장 아래쪽 블럭 위치이다
                for (int nRow = 0; nRow < m_nRow; nRow++)
                {
                    if (CanBlockBeAllocatable(nRow, nCol))
                        emptyBlocks.Add(nRow, nRow);
                }

                //아래쪽에 비었는 블럭이 없는 경	
                if (emptyBlocks.Count == 0)
                    continue;

                //2. 이동이 가능한 블럭을 비어있는 하단 위치로 이동한다.

                //2.1 가장 아래쪽부터 비어있는 블럭을 처리한다
                IntIntKV first = emptyBlocks.First();

                //2.2 비어있는 블럭 위쪽 방향으로 이동 가능한 블럭을 탐색하면서 빈 블럭을 채워나간다
                for (int nRow = first.Value + 1; nRow < m_nRow; nRow++)
                {
                    Block block = m_Blocks[nRow, nCol];

                    //2.2.1 이동 가능한 아이템이 아닌 경우 pass
                    if (block == null || m_Cells[nRow, nCol].type == CellType.EMPTY) //TODO EMPTY를 직접체크하지 않고 이러한 부류를 함수로 체크
                        continue;

                    //2.2.2 드롭을 방해하는 cell이 있는 경우 해당 cell 아래쪽은 비워둔다 (직전방향 드롭하지 않음) 
                    //      아래쪽 비어있는 블럭은 이동가능한 목록에서 제거한다

                    //2.2.3 이동이 필요한 블럭 발견
                    block.dropDistance = new Vector2(0, nRow - first.Value);    //GameObject 애니메이션 이동
                    movingBlocks.Add(block);

                    //2.2.4 빈 공간으로 이동
                    Debug.Assert(m_Cells[first.Value, nCol].IsObstracle() == false, $"{m_Cells[first.Value, nCol]}");
                    m_Blocks[first.Value, nCol] = block;        // 이동될 위치로 Board에서 저장된 위치 이동

                    //2.2.5 다른 곳으로 이동했음으로 현재 위치는 비워둔다
                    m_Blocks[nRow, nCol] = null;

                    //2.2.6 비어있는 블럭 리스트에서 사용된 첫번째 노드(first)를 삭제한다
                    emptyBlocks.RemoveAt(0);

                    //2.2.7 현재 위치의 블럭이 다른 위치로 이동했으므로 현재 위치가 비어있게 된다.
                    //그러므로 비어있는 블럭을 보관하는 emptyBolocks에 추가한다
                    emptyBlocks.Add(nRow, nRow);

                    //2.2.8 다음(Next) 비어었는 블럭을 처리하도록 기준을 변경한다
                    first = emptyBlocks.First();
                    nRow = first.Value; //Note : 빈곳 바로 위부터 처리하도록 위치 조정, for 문에서 nRow++ 하기 때문에 +1을 하지 않는다
                }
            }

            yield return null;

            //드롭으로 채워지지 않는 블럭이 있는 경우(왼쪽 아래 순으로 들어있음)
            if (emptyRemainBlocks.Count > 0)
            {
                unfilledBlocks.AddRange(emptyRemainBlocks);
            }

            yield break;
        }

        /*
         * 비어있는 블럭 다시 생성해서 전체 보드를 다시 구성한다
         */
        public IEnumerator SpawnBlocksAfterClean(List<Block> movingBlocks)
        {
            for (int nCol = 0; nCol < m_nCol; nCol++)
            {
                for (int nRow = 0; nRow < m_nRow; nRow++)
                {
                    //비어있는 블럭이 있는 경우, 상위 열은 모두 비어있거나, 장애물로 인해서 남아있음.
                    if (m_Blocks[nRow, nCol] == null)
                    {
                        int nTopRow = nRow;
                        int nSpawnBaseY = 0;

                        for (int y = nTopRow; y < m_nRow; y++)
                        {
                            if (m_Blocks[y, nCol] != null || !CanBlockBeAllocatable(y, nCol))
                                continue;

                            Block block = SpawnBlockWithDrop(y, nCol, nSpawnBaseY, nCol);
                            if (block != null)
                                movingBlocks.Add(block);

                            nSpawnBaseY++;
                        }

                        break;
                    }
                }
            }

            yield return null;
        }

        /*
         * 블럭을 생성하고 목적지(nRow, nCol) 까지 드롭한다
         * @param nRow, nCol : 생성후 보드에 저장되는 위치
         * @param nSpawnedRow, nSpawnedCol : 화면에 생성되는 위치, nRow, nCol 위치까지 드롭 액션이 연출된다
         */
        Block SpawnBlockWithDrop(int nRow, int nCol, int nSpawnedRow, int nSpawnedCol)
        {
            float fInitX = CalcInitX(Core.Constants.BLOCK_ORG);
            float fInitY = CalcInitY(Core.Constants.BLOCK_ORG) + m_nRow;

            Block block = m_StageBuilder.SpawnBlock().InstantiateBlockObj(m_BlockPrefab, m_Container);
            if (block != null)
            {
                m_Blocks[nRow, nCol] = block;
                block.Move(fInitX + (float)nSpawnedCol, fInitY + (float)(nSpawnedRow));
                block.dropDistance = new Vector2(nSpawnedCol - nCol, m_nRow + (nSpawnedRow - nRow));
            }

            return block;
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

        /*
         * 블럭이 지정된 위치에 새로 할당 될 수 있는지 체크한다
         */
        bool CanBlockBeAllocatable(int nRow, int nCol)
        {
            if (!m_Cells[nRow, nCol].type.IsBlockAllocatableType())
                return false;

            return m_Blocks[nRow, nCol] == null;
        }
    }
}
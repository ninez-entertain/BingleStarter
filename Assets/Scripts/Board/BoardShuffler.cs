using System.Collections.Generic;
using UnityEngine;
using Ninez.Core;

namespace Ninez.Board
{
    using BlockVectorKV = KeyValuePair<Block, Vector2Int>;

    public class BoardShuffler
    {
        Board m_Board;
        bool m_bLoadingMode;

        SortedList<int, BlockVectorKV> m_OrgBlocks = new SortedList<int, BlockVectorKV>();
        IEnumerator<KeyValuePair<int, BlockVectorKV>> m_it;
        Queue<BlockVectorKV> m_UnusedBlocks = new Queue<BlockVectorKV>();
        bool m_bListComplete;

        public BoardShuffler(Board board, bool bLoadingMode)
        {
            m_Board = board;
            m_bLoadingMode = bLoadingMode;
        }

        public void Shuffle(bool bAnimation = false)
        {
            //1. 셔플 대비해서 각 블럭의 매칭 정보를 업데이트한다
            PrepareDuplicationDatas();

            //2. 셔플 대상 블럭을 별도 리스트에 보관한다
            PrepareShuffleBlocks();

            //3. 1), 2)에서 준비한 데이터를 이용하여 셔플을 수행한다.
            RunShuffle(bAnimation);
        }

        BlockVectorKV NextBlock(bool bUseQueue)
        {
            if (bUseQueue && m_UnusedBlocks.Count > 0)
                return m_UnusedBlocks.Dequeue();

            if (!m_bListComplete && m_it.MoveNext())
                return m_it.Current.Value;

            m_bListComplete = true;

            return new BlockVectorKV(null, Vector2Int.zero);
        }

        /*
         * 전체 블럭의 중복정보를 계산한다. 
         * 중복정보를 스테이지 시작시 또는 shuffle시 각 블럭에 해당 정보가 저장한다.
         * 셔플이 대상 블럭은 0으로 초기화 되고, 미대상 블럭은 중복정보를 업데이트한다        
         */
        void PrepareDuplicationDatas()
        {
            for (int nRow = 0; nRow < m_Board.maxRow; nRow++)
                for (int nCol = 0; nCol < m_Board.maxCol; nCol++)
                {
                    Block block = m_Board.blocks[nRow, nCol];

                    if (block == null)
                        continue;

                    if (m_Board.CanShuffle(nRow, nCol, m_bLoadingMode))
                        block.ResetDuplicationInfo();
                    //움직이지 못하는 블럭(밧줄에 묶인 경우 등 현재상태에서 이동불가한 블럭)의 매칭 정보를 계산한다.
                    else
                    {
                        block.horzDuplicate = 1;
                        block.vertDuplicate = 1;

                        //좌하 위치에 셔플 미대상(즉, 움직이지 못하는 블럭)인 블럭의 매치 상태를 반영한다.
                        //(3개이상 매치되는 경우는 발생하지 않기 때문에 인접한 블럭만 검사하면 된다)
                        //Note : 좌하만 계산해도 전체 블럭을 모두 검사할 수 있다
                        if (nCol > 0 && !m_Board.CanShuffle(nRow, nCol - 1, m_bLoadingMode) && m_Board.blocks[nRow, nCol - 1].IsSafeEqual(block))
                        {
                            block.horzDuplicate = 2;
                            m_Board.blocks[nRow, nCol - 1].horzDuplicate = 2;
                        }

                        if (nRow > 0 && !m_Board.CanShuffle(nRow - 1, nCol, m_bLoadingMode) && m_Board.blocks[nRow - 1, nCol].IsSafeEqual(block))
                        {
                            block.vertDuplicate = 2;
                            m_Board.blocks[nRow - 1, nCol].vertDuplicate = 2;
                        }
                    }
                }
        }

        /* 
         * 셔플 대상 블럭을 별도의 Sorted 리스트에 보관한다.
         * 랜덤값에 따라 블럭이 정렬되면서 섞인다.
         */
        void PrepareShuffleBlocks()
        {
            for (int nRow = 0; nRow < m_Board.maxRow; nRow++)
                for (int nCol = 0; nCol < m_Board.maxCol; nCol++)
                {
                    if (!m_Board.CanShuffle(nRow, nCol, m_bLoadingMode))
                        continue;

                    //Sorted List에 순서를 정하기 위해서 중복값이 없도록 랜덤 값을 생성한후 키값으로 저장한다
                    while (true)
                    {
                        int nRandom = UnityEngine.Random.Range(0, 10000);
                        //detect key duplication
                        if (m_OrgBlocks.ContainsKey(nRandom))
                            continue;

                        m_OrgBlocks.Add(nRandom, new BlockVectorKV(m_Board.blocks[nRow, nCol], new Vector2Int(nCol, nRow)));
                        break;
                    }
                }

            m_it = m_OrgBlocks.GetEnumerator();
        }

        /*
         * 전체 블럭을 섞는다
         */
        void RunShuffle(bool bAnimation)
        {
            for (int nRow = 0; nRow < m_Board.maxRow; nRow++)
            {
                for (int nCol = 0; nCol < m_Board.maxCol; nCol++)
                {
                    if (!m_Board.CanShuffle(nRow, nCol, m_bLoadingMode))
                        continue;

                    m_Board.blocks[nRow, nCol] = GetShuffledBlock(nRow, nCol);
                }
            } 
        } 

        /**
         * 지정된 위치에 배치할 수 있는 3매치되지 않는 블럭을 한다.
         */
        Block GetShuffledBlock(int nRow, int nCol)
        {
            BlockBreed prevBreed = BlockBreed.NA;   //처음 비교시에 종류를 저장
            Block firstBlock = null;                //리스트를 전부 처리하고 큐만 남은 경우에 중복 체크 위해 사용 (큐에서 꺼낸 첫번째 블럭)

            bool bUseQueue = true;  //true : 큐에서 꺼냄, false : 리스트에서 꺼냄
            while (true)
            {
                //1. Queue에서 블럭을 하나 꺼낸다. 첫번재 후보이다.
                BlockVectorKV blockInfo = NextBlock(bUseQueue);
                Block block = blockInfo.Key;

                //2. 리스트에서 블럭을 전부 처리한 경우 : 전체 루프(for 문 포함)에서 1회만 발생
                if (block == null)
                {
                    blockInfo = NextBlock(true);
                    block = blockInfo.Key;
                }

                Debug.Assert(block != null, $"block can't be null : queue  count -> {m_UnusedBlocks.Count}");

                if (prevBreed == BlockBreed.NA) //첫비교시 종류 저장
                    prevBreed = block.breed;

                //3. 리스트를 모두 처리 한 경우
                if (m_bListComplete)
                {
                    if (firstBlock == null)
                    {
                        //3.1 전체 리스트를 처리하고, 처음으로 큐에서 꺼낸 경우
                        firstBlock = block;  // 큐에서 꺼낸 첫번째 블럭
                    }
                    else if (System.Object.ReferenceEquals(firstBlock, block))
                    {
                        //3.2 처음 보았던 블럭을 다시 처리하는 경우, 
                        //    즉, 큐에 들어있는 모든 블럭이 조건에 맞지 않는 경우 (남은 블럭 중에 조건에 맞는게 없는 경우)
                        m_Board.ChangeBlock(block, prevBreed);
                    }
                }

                //4. 상하좌우 인접 블럭과 겹치는 개수를 계산한다
                Vector2Int vtDup = CalcDuplications(nRow, nCol, block);

                //5. 2개 이상 매치되는 경우, 현재 위치에 해당 블럭이 올 수 없으므로 큐에 보관하고 다음 블럭 처리하도록 continue한다
                if (vtDup.x > 2 || vtDup.y > 2)
                {
                    m_UnusedBlocks.Enqueue(blockInfo);
                    bUseQueue = m_bListComplete || !bUseQueue;

                    continue;
                }

                //6. 블럭이 위치할 수 있는 경우, 찾은 위치로 Bock GameObject를 이동시킨다.
                block.vertDuplicate = vtDup.y;
                block.horzDuplicate = vtDup.x;
                if (block.blockObj != null)
                {
                    float initX = m_Board.CalcInitX(Constants.BLOCK_ORG);
                    float initY = m_Board.CalcInitY(Constants.BLOCK_ORG);
                    block.Move(initX + nCol, initY + nRow);
                }

                //7. 찾은 블럭을 리턴한다.
                return block;
            }
        }

        /**
         * 상하좌우 인접 블럭과 겹치는 개수를 계산한다
         */
        Vector2Int CalcDuplications(int nRow, int nCol, Block block)
        {
            int colDup = 1, rowDup = 1;

            if (nCol > 0 && m_Board.blocks[nRow, nCol - 1].IsSafeEqual(block))
                colDup += m_Board.blocks[nRow, nCol - 1].horzDuplicate;

            if (nRow > 0 && m_Board.blocks[nRow - 1, nCol].IsSafeEqual(block))
                rowDup += m_Board.blocks[nRow - 1, nCol].vertDuplicate;

            if (nCol < m_Board.maxCol - 1 && m_Board.blocks[nRow, nCol + 1].IsSafeEqual(block))
            {
                Block rightBlock = m_Board.blocks[nRow, nCol + 1];
                colDup += rightBlock.horzDuplicate;

                //셔플 미대상블럭이 현재 블럭과 중복되는 경우, 셔플미대상 블럭의 중복 정보도 함게 업데이트한다
                if (rightBlock.horzDuplicate == 1)
                    rightBlock.horzDuplicate = 2;
            }

            if (nRow < m_Board.maxRow - 1 && m_Board.blocks[nRow + 1, nCol].IsSafeEqual(block))
            {
                Block upperBlock = m_Board.blocks[nRow + 1, nCol];
                rowDup += upperBlock.vertDuplicate;

                //셔플 미대상블럭이 현재 블럭과 중복되는 경우, 셔플미대상 블럭의 중복 정보도 함게 업데이트한다
                if (upperBlock.vertDuplicate == 1)
                    upperBlock.vertDuplicate = 2;
            }

            return new Vector2Int(colDup, rowDup);
        }
    }
}

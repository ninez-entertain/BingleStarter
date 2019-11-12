using System;
using UnityEngine;
using Ninez.Board;

namespace Ninez.Stage
{
    public class StageBuilder
    {
        StageInfo m_StageInfo;
        int m_nStage;

        public StageBuilder(int nStage)
        {
            m_nStage = nStage;
        }

        /// <summary>
        /// 주어진 크기의 Stage를 생성하고,  Stage를 구성하는 보드의 Cell과 Block을 구성한다
        /// </summary>
        /// <returns>생성된 Stage 객체</returns>
        public Stage ComposeStage()
        {
            Debug.Assert(m_nStage > 0, $"Invalide Stage : {m_nStage}");

            //0. 스테이지 정보를 로드한다.(보드 크기, Cell/블럭 정보 등)
            m_StageInfo = LoadStage(m_nStage);

            //1. Stage 객체를 생성한다.
            Stage stage = new Stage(this, m_StageInfo.row, m_StageInfo.col);

            //2. Cell,Block 초기 값을 생성한다.
            for (int nRow = 0; nRow < m_StageInfo.row; nRow++)
            {
                for (int nCol = 0; nCol < m_StageInfo.col; nCol++)
                {
                    stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                    stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
                }
            }

            return stage;
        }

        /// <summary>
        /// 스테이지 구성을 위해서 구성정보를 로드한다. 
        /// </summary>
        /// <param name="nStage">스테이지 번</param>
        public StageInfo LoadStage(int nStage)
        {
            StageInfo stageInfo = StageReader.LoadStage(nStage);
            if (stageInfo != null)
            {
                Debug.Log(stageInfo.ToString());
            }

            return stageInfo;
        }

        /// <summary>
        /// 지정된 위치에 적합한 Block 객체를 생성한다. 
        /// </summary>
        /// <param name="nRow">행</param>
        /// <param name="nCol">열</param>
        /// <returns></returns>
        Block SpawnBlockForStage(int nRow, int nCol)
        {
            if (m_StageInfo.GetCellType(nRow, nCol) == CellType.EMPTY)
                return SpawnEmptyBlock();

            return SpawnBlock();
        }

        /// <summary>
        /// 지정된 위치에 적합한 Cell 객체를 생성한다.
        /// </summary>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        /// <returns></returns>
        Cell SpawnCellForStage(int nRow, int nCol)
        {
            Debug.Assert(m_StageInfo != null);
            Debug.Assert(nRow < m_StageInfo.row && nCol < m_StageInfo.col);

            return CellFactory.SpawnCell(m_StageInfo, nRow, nCol);
        }

        /// <summary>
        /// 주어진 정보를 이용해서 StageBuilder를 생성하고, 보드 크기에 해당하는 Stage를 생성한다.
        /// </summary>
        /// <param name="nStage"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Stage BuildStage(int nStage)
        {
            StageBuilder stageBuilder = new StageBuilder(nStage);
            Stage stage = stageBuilder.ComposeStage();

            return stage;
        }

        /// <summary>
        /// 기본형 블럭을 요청한다.
        /// </summary>
        /// <returns>생성된 Block 객체</returns>
        public Block SpawnBlock()
        {
            return BlockFactory.SpawnBlock(BlockType.BASIC);
        }
        
        /// <summary>
        /// BlockType.EMPTY인 블럭을 요청한다
        /// </summary>
        /// <returns>생성된 Block 객체</returns>
        public Block SpawnEmptyBlock()
        {
            Block newBlock = BlockFactory.SpawnBlock(BlockType.EMPTY);

            return newBlock;
        }


    }
}
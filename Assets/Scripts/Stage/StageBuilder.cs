using System;
using UnityEngine;
using Ninez.Board;

namespace Ninez.Stage
{
    public class StageBuilder
    {
        int m_nStage;

        public StageBuilder(int nStage)
        {
            m_nStage = nStage;
            //LoadStage(nStage);
        }

        /// <summary>
        /// 주어진 크기의 Stage를 생성하고,  Stage를 구성하는 보드의 Cell과 Block을 구성한다
        /// </summary>
        /// <param name="row"></param> 블럭을 구성하는 '행' 개수
        /// <param name="col"></param> 블럭을 구성하는 '열' 개순
        /// <returns>Stage 객</returns>
        public Stage ComposeStage(int row, int col)
        {
            //1. Stage 객체를 생성한다.
            Stage stage = new Stage(this, row, col);

            //2. Cell,Block 초기 값을 생성한다.
            for (int nRow = 0; nRow < row; nRow++)
            {
                for (int nCol = 0; nCol < col; nCol++)
                {
                    stage.blocks[nRow, nCol] = SpawnBlockForStage(nRow, nCol);
                    stage.cells[nRow, nCol] = SpawnCellForStage(nRow, nCol);
                }
            }

            return stage;
        }


        Block SpawnBlockForStage(int nRow, int nCol)
        {
            return new Block(BlockType.BASIC);
            //return new Block(nRow == nCol ? BlockType.EMPTY : BlockType.BASIC);
        }

        Cell SpawnCellForStage(int nRow, int nCol)
        {
            return new Cell(nRow == nCol ? CellType.EMPTY : CellType.BASIC);
        }

        /// <summary>
        /// 주어진 정보를 이용해서 StageBuilder를 생성하고, 보드 크기에 해당하는 Stage를 생성한다.
        /// </summary>
        /// <param name="nStage"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static Stage BuildStage(int nStage, int row, int col)
        {
            StageBuilder stageBuilder = new StageBuilder(0);
            Stage stage = stageBuilder.ComposeStage(9, 9);

            return stage;
        }

    }
}
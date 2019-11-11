using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Board;
using System;

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
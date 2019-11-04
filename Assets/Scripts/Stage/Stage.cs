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
        /// 
        /// </summary>
        /// <param name="stageBuilder"></param>
        /// <param name="nRow"></param>
        /// <param name="nCol"></param>
        public Stage(StageBuilder stageBuilder, int nRow, int nCol)
        {
            m_StageBuilder = stageBuilder;

            m_Board = new Ninez.Board.Board(nRow, nCol);
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
                    strBlocks.Append($"{blocks[nRow, nCol].type}, ");
                }

                strCells.Append("\n");
                strBlocks.Append("\n");
            }

            Debug.Log(strCells.ToString());
            Debug.Log(strBlocks.ToString());
        }
    }
}
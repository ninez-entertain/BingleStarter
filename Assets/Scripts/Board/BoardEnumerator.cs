using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Board;

namespace Ninez.Board
{
    public class BoardEnumerator
    {
        Ninez.Board.Board m_Board;

        public BoardEnumerator(Ninez.Board.Board board)
        {
            this.m_Board = board;
        }

        public bool IsCageTypeCell(int nRow, int nCol)
        {
            return false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Board;

namespace Ninez.Stage
{
    public class Stage
    {
        int m_nRow;
        int m_nCol;
        public int maxRow { get { return m_nRow; } }
        public int maxCol { get { return m_nCol; } }

        Ninez.Board.Board m_Board;
        public Ninez.Board.Board board { get { return m_Board; } }
    }
}
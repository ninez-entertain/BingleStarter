using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Board
{
    public class Cell
    {
        protected CellType m_CellType;
        public CellType type
        {
            get { return m_CellType; }
            set { m_CellType = value; }
        }
    }
}
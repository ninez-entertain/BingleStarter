using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Board
{
    public class Block
    {
        BlockType m_BlockType;
        public BlockType type
        {
            get { return m_BlockType; }
            set { m_BlockType = value; }
        }
    }
}
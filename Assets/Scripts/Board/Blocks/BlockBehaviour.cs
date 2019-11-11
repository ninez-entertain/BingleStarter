using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Scriptable;

namespace Ninez.Board
{
    public class BlockBehaviour : MonoBehaviour
    {
        Block m_Block;
        SpriteRenderer m_SpriteRenderer;
        [SerializeField] BlockConfig m_BlockConfig;

        void Start()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();

            UpdateView(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        internal void SetBlock(Block block)
        {
            m_Block = block;
        }

        /// <summary>
        /// 참조하고 있는 Block의 보를 반영하여 Block GameObject에 반영한다
        /// ex) Block 종류에 따른 Sprite 종류 업데이트
        /// 생성자 또는 플레이도중에 Block Type이 변경될 때 호출된다.
        /// </summary>
        /// <param name="bValueChanged">플레이 도중에 Type이 변경되는 경우 true, 그렇지 않은 경우 false</param>
        public void UpdateView(bool bValueChanged)
        {
            if (m_Block.type == BlockType.EMPTY)
            {
                m_SpriteRenderer.sprite = null;
            }
            else if(m_Block.type == BlockType.BASIC)
            {
                m_SpriteRenderer.sprite = m_BlockConfig.basicBlockSprites[(int)m_Block.breed];
            }
        }
    }
}
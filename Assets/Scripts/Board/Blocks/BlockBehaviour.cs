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

        /*
         * 블럭을 제거한다.  
         */
        public void DoActionClear()
        {
            StartCoroutine(CoStartSimpleExplosion(true));
        }

        /*
         * 블럭이 폭발한 후, GameObject를 삭제한다.
         */
        IEnumerator CoStartSimpleExplosion(bool bDestroy = true)
        {
            //1. 크기가 줄어드는 액션 실행한다 : 폭파되면서 자연스럽게 소멸되는 모양 연출, 1 -> 0.3으로 줄어든다.
            yield return Util.Action2D.Scale(transform, Core.Constants.BLOCK_DESTROY_SCALE, 4f);

            //2. 폭파시키는 효과 연출 : 블럭 자체의 Clear 효과를 연출한다 (모든 블럭 동일)
            GameObject explosionObj = m_BlockConfig.GetExplosionObject(BlockQuestType.CLEAR_SIMPLE);
            ParticleSystem.MainModule newModule = explosionObj.GetComponent<ParticleSystem>().main;
            newModule.startColor = m_BlockConfig.GetBlockColor(m_Block.breed);

            explosionObj.SetActive(true);
            explosionObj.transform.position = this.transform.position;

            yield return new WaitForSeconds(0.1f);

            //3. 블럭 GameObject 객체 삭제 or make size zero
            if (bDestroy)
                Destroy(gameObject);
            else
            {
                Debug.Assert(false, "Unknown Action : GameObject No Destory After Particle");
            }
        }

    }
}
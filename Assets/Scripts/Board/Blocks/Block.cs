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

        protected BlockBreed m_Breed;   //렌더링되는 블럭 캐린터(즉, 이미지 종류)
        public BlockBreed breed
        {
            get { return m_Breed; }
            set
            {
                m_Breed = value;
                m_BlockBehaviour?.UpdateView(true);
            }
        }

        protected BlockBehaviour m_BlockBehaviour;
        public BlockBehaviour blockBehaviour
        {
            get { return m_BlockBehaviour; }
            set
            {
                m_BlockBehaviour = value;
                m_BlockBehaviour.SetBlock(this);
            }
        }

        public Block(BlockType blockType)
        {
            m_BlockType = blockType;
        }

        /// <summary>
        /// 블럭을 디스플레이하는 GameObject를 생성한다. 출력이 필요한 경우에만 생성한다.
        /// 
        /// </summary>
        /// <param name="blockPrefab"></param>
        /// <param name="containerObj"></param>
        /// <returns>return 비어있는 블럭인 경우 null, 유효한 경우 현재 block</returns>
        internal Block InstantiateBlockObj(GameObject blockPrefab, Transform containerObj)
        {
            //유효하지 않은 블럭인 경우, Block GameObject를 생성하지 않는다.
            if (IsValidate() == false)
                return null;

            //1. Block 오브젝트를 생성한다.
            GameObject newObj = Object.Instantiate(blockPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            //2. 컨테이너(Board)의 차일드로 Block을 포함시킨다.
            newObj.transform.parent = containerObj;

            //3. Block 오브젝트에 적용된 BlockBehaviour 컴포너트를 보관한다.
            this.blockBehaviour = newObj.transform.GetComponent<BlockBehaviour>();

            return this;
        }

        /// <summary>
        /// 지정된 위치로 Bloc GameObject위 위치(Position)을 변경한다
        /// </summary>
        /// <param name="x">X 좌표 : 씬 기준</param>
        /// <param name="y">Y 좌표 : 씬 기준</param>
        internal void Move(float x, float y)
        {
            blockBehaviour.transform.position = new Vector3(x, y);
        }

        /// <summary>
        /// 유효한 블럭인지 체크한다.
        /// EMPTY 타입을 제외하고 모든 블럭이 유효한 것으로 간주한다.
        /// Block GameObject 생성 등의 판단에 사용된다.
        /// </summary>
        /// <returns></returns>
        public bool IsValidate()
        {
            return type != BlockType.EMPTY;
        }
    }
}
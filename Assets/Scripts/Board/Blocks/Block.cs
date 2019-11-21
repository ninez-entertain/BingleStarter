using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Quest;

namespace Ninez.Board
{
    public class Block
    {
        //---------------------------------------------------------------------
        // Members
        //---------------------------------------------------------------------
        public BlockStatus status;
        public BlockQuestType questType;

        public MatchType match = MatchType.NONE;

        public short matchCount;

        //---------------------------------------------------------------------
        // Properties 
        //---------------------------------------------------------------------
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

        public Transform blockObj { get { return m_BlockBehaviour?.transform; } }

        Vector2Int m_vtDuplicate;       // 블럭 젠, Shuffle시에 중복검사에 사용. stage file에서 생성시 (-1, -1)
        //중복 검사시 사용 
        public int horzDuplicate
        {
            get { return m_vtDuplicate.x; }
            set { m_vtDuplicate.x = value; }
        }

        //중복 검사시 사용 
        public int vertDuplicate
        {
            get { return m_vtDuplicate.y; }
            set { m_vtDuplicate.y = value; }
        }

        int m_nDurability;                 //내구도
        public virtual int durability
        {
            get { return m_nDurability; }
            set { m_nDurability = value; }
        }

        //---------------------------------------------------------------------
        // Constructor
        //---------------------------------------------------------------------

        public Block(BlockType blockType)
        {
            m_BlockType = blockType;

            status = BlockStatus.NORMAL;
            questType = BlockQuestType.CLEAR_SIMPLE;
            match = MatchType.NONE;
            m_Breed = BlockBreed.NA;

            m_nDurability = 1;
        }

        //---------------------------------------------------------------------
        // Methods
        //---------------------------------------------------------------------

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

        /*
         * 블럭의 컨텍스트(종류, 상태, 카운트, 퀘스트조건 등)에 해당하는 동작을 수행하도록 한다.
         * return 특수블럭이면 true, 그렇지 않으면 false    
         */
        public bool DoEvaluation(BoardEnumerator boardEnumerator, int nRow, int nCol)
        {
            //매칭 로직 임시 적용, Qeust 처리 별도 클래스 분리 필요

            Debug.Assert(boardEnumerator != null, $"({nRow},{nCol})");

            if (!IsEvaluatable())
                return false;

            //1. 매치 상태(클리어 조건 충족)인 경우
            if (status == BlockStatus.MATCH)
            {
                if (questType == BlockQuestType.CLEAR_SIMPLE || boardEnumerator.IsCageTypeCell(nRow, nCol)) //TODO cagetype cell 조건이 필요한가? 
                {
                    Debug.Assert(m_nDurability > 0, $"durability is zero : {m_nDurability}");

                    durability--;
                }
                else //특수블럭
                {
                    return true;
                }

                if (m_nDurability == 0)
                {
                    status = BlockStatus.CLEAR;
                    return false;
                }
            }

            //2. 클리어 조건에 아직 도달하지 않는 경우 NORMAL 상태로 복귀
            status = BlockStatus.NORMAL;
            match = MatchType.NONE;
            matchCount = 0;

            return false;
        }

        /*
         * 블럭의 상태를 MATCH 로 설정한다.
         * @param bAccumulate 기존값에 더해진 값으로 누적되는 경우 true    
         */
        public void UpdateBlockStatusMatched(MatchType matchType, bool bAccumulate = true)
        {
            this.status = BlockStatus.MATCH;

            if (match == MatchType.NONE)
            {
                this.match = matchType;
            }
            else
            {
                this.match = bAccumulate ? match.Add(matchType) : matchType; //match + matchType
            }

            matchCount = (short)matchType;
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

        public void MoveTo(Vector3 to, float duration)
        {
            m_BlockBehaviour.StartCoroutine(Util.Action2D.MoveTo(blockObj, to, duration));
        }

        public virtual void Destroy()
        {
            Debug.Assert(blockObj != null, $"{match}");
            blockBehaviour.DoActionClear();
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

        public void ResetDuplicationInfo()
        {
            m_vtDuplicate.x = 0;
            m_vtDuplicate.y = 0;
        }

        /// <summary>
        /// target Block과 같은 breed를 가지고 있는지 검사한다.
        /// </summary>
        /// <param name="target">비교할 대상 Block</param>
        /// <returns>breed가 같으면 true, 다르면 false</returns>
        public bool IsEqual(Block target)
        {
            if (IsMatchableBlock() && this.breed == target.breed)
                return true;

            return false;
        }

        /*
         * 3 매칭으로 제거 가능한 블럭인지 검사한다.
         */
        public bool IsMatchableBlock()
        {
            return !(type == BlockType.EMPTY);
        }

        /*
         * swipe 가능한 블럭인지 체크한다
         * @param baseBlock 스와이프 기준 블럭, 기준블럭 종류에 따라서 가능 여부가 달라진다    
         */
        public bool IsSwipeable(Block baseBlock)
        {
            return true;
        }

        /*
         * Evaluation 대상에 적합한지 체크한다
         */
        public bool IsEvaluatable()
        {
            //이미 처리완료(CLEAR) 되었거나, 현재 처리중인 블럭인 경우
            if (status == BlockStatus.CLEAR || !IsMatchableBlock())
                return false;

            return true;
        }
    }
}
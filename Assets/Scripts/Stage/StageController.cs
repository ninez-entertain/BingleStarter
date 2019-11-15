using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ninez.Util;

namespace Ninez.Stage
{
    public class StageController : MonoBehaviour
    {
        bool m_bInit;
        Stage m_Stage;
        InputManager m_InputManager;

        //Event Members
        bool m_bTouchDown;
        Vector2Int m_BlockDownPos;  //블럭 인덱스 (보드에 저장된 위치)
        Vector3 m_ClickPos;         //World 좌표

        [SerializeField] Transform m_Container;
        [SerializeField] GameObject m_CellPrefab;
        [SerializeField] GameObject m_BlockPrefab;

        void Start()
        {
            InitStage();
        }

        private void Update()
        {
            if (!m_bInit)
                return;

            OnInputHandler();
        }

        void InitStage()
        {
            if (m_bInit)
                return;

            m_bInit = true;
            m_InputManager = new InputManager(m_Container);

            BuildStage();

            //m_Stage.PrintAll();
        }

        /*
         * 스테이지를 구성한다.
         * Stage 객체를 할당받고, Stage 구성을 요청한다.
         */
        void BuildStage()
        {
            //1. Stage를 구성한다.
            m_Stage = StageBuilder.BuildStage(nStage : 2);

            //2. 생성한 stage 정보를 이용하여 씬을 구성한.
            m_Stage.ComposeStage(m_CellPrefab, m_BlockPrefab, m_Container);
        }

        void OnInputHandler()
        {
            //Touch Down 
            if (m_InputManager.isTouchDown)
            {
                Vector2 point = m_InputManager.touchPosition;

                //Play 영역에서 클릭하지 않는 경우는 무시
                if (!IsInsideBoard(point))
                    return;

                Vector2Int blockPos;
                if(m_Stage.IsOnValideBlock(point, out blockPos))
                {
                    m_bTouchDown = true;
                    m_BlockDownPos = blockPos;
                    m_ClickPos = point;
                    //Debug.Log($"Mouse Down In Board : (row = {blockPos.y}, col = {blockPos.x})");
                }
            }
            //Touch UP : 유효한 블럭 위에서 Down 후에 발생하는 경우
            else if (m_bTouchDown && m_InputManager.isTouchUp)
            {
                m_bTouchDown = false;

                Vector2 point = m_InputManager.touchPosition;

                SwipeDir swipeDir = TouchEvaluator.EvalSwipeDir(m_ClickPos, point);

                Debug.Log($"Swipe : {swipeDir} , Block = ({m_BlockDownPos.y}, {m_BlockDownPos.x})");
            }
        }

        /*
         * 보드안에서 발생한 이벤트인지 체크한다       
         * @param point container ralative point (no screen point)       
         */
        bool IsInsideBoard(Vector2 point)
        {
            return m_Stage.IsInsideBoard(point);
        }
    }
}

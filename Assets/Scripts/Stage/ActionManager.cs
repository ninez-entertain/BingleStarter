using System.Collections;
using System.Collections.Generic;
using Ninez.Util;
using UnityEngine;

namespace Ninez.Stage
{
    /**
     * 플레이어의 액션을 처리하는 클래스
     */
    public class ActionManager 
    {
        Transform m_Container;          //컨테이저 (Board GameObject)
        Stage m_Stage;                  
        MonoBehaviour m_MonoBehaviour;  //코루틴 호출시 필요한 MonoBehaviour

        bool m_bRunning;                //액션 실행 상태 : 실행중인 경우 true

        public ActionManager(Transform container, Stage stage)
        {
            m_Container = container;
            m_Stage = stage;

            m_MonoBehaviour = container.gameObject.GetComponent<MonoBehaviour>();
        }

        /*
         * 코루틴 Wapper 메소드   
         */
        public Coroutine StartCoroutine(IEnumerator routine)
        {
            return m_MonoBehaviour.StartCoroutine(routine);
        }

        /*
         * 스와이프를 액션을 시작한다.
         * @param nRow, nCol 블럭 위치
         * @swipeDir 스와이프 방향
         */
        public void DoSwipeAction(int nRow, int nCol, Swipe swipeDir)
        {
            Debug.Assert(nRow >= 0 && nRow < m_Stage.maxRow && nCol >= 0 && nCol < m_Stage.maxCol);

            if (m_Stage.IsValideSwipe(nRow, nCol, swipeDir))
            {
                StartCoroutine(CoDoSwipeAction(nRow, nCol, swipeDir));
            }
        }

        /*
         * 스와이프 액션을 수행하는 코루틴
         */
        IEnumerator CoDoSwipeAction(int nRow, int nCol, Swipe swipeDir)
        {
            if (!m_bRunning)  //다른 액션이 수행 중이면 PASS
            {
                m_bRunning = true;    //액션 실행 상태 ON

                //1. swipe action 수행
                Returnable<bool> bSwipedBlock = new Returnable<bool>(false);
                yield return m_Stage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipedBlock);

                m_bRunning = false;  //액션 실행 상태 OFF
            }
            yield break;
        }
    }
}
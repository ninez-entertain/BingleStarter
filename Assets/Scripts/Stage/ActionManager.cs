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

                SoundManager.instance.PlayOneShot(Clip.Chomp);

                //1. swipe action 수행
                Returnable<bool> bSwipedBlock = new Returnable<bool>(false);
                yield return m_Stage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipedBlock);

                //2. 스와이프 성공한 경우 브드를 평가(매치블럭삭제, 빈블럭 드롭, 새블럭 Spawn 등)한다.
                if (bSwipedBlock.value)
                {
                    Returnable<bool> bMatchBlock = new Returnable<bool>(false);
                    yield return EvaluateBoard(bMatchBlock);

                    //스와이프한 블럭이 매치되지 않은 경우에 원상태 복귀
                    if (!bMatchBlock.value)
                    {
                        yield return m_Stage.CoDoSwipeAction(nRow, nCol, swipeDir, bSwipedBlock);
                    }
                }

                m_bRunning = false;  //액션 실행 상태 OFF
            }
            yield break;
        }

        /*
         * 현상태에서 보드를 평가한다. 즉 보드를 구성하는 블럭에 게임규칙을 적용시킨다.
         * 매치된 블럭은 제거하고 빈자리에는 새로운 블럭을 생성한다.    
         * matchResult : 실행 결과를 리턴받은 클래스 
         *               true : 매치된 블럭 있는 경우, false : 없는 경우
         */
        IEnumerator EvaluateBoard(Returnable<bool> matchResult)
        {
            while (true)    //매칭된 블럭이 있는 경우 반복 수행한다.
            {
                //1. 매치 블럭 제거
                Returnable<bool> bBlockMatched = new Returnable<bool>(false);
                yield return StartCoroutine(m_Stage.Evaluate(bBlockMatched));

                //2. 3매치 블럭이 있는 경우 후처리 싱행 (블럭 드롭 등)
                if (bBlockMatched.value)
                {
                    matchResult.value = true;

                    SoundManager.instance.PlayOneShot(Clip.BlcokClear);

                    // 매칭 블럭 제거 후 빈블럭 드롭 후 새 블럭 생성
                    yield return StartCoroutine(m_Stage.PostprocessAfterEvaluate());
                }
                //3. 3매치 블럭이 없는 경우 while 문 종료
                else
                    break;  
            }

            yield break;
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Util
{
    public class InputManager
    {
        Transform m_Container;

#if UNITY_ANDROID && !UNITY_EDITOR
        IInputHandlerBase m_InputHandler = new TouchHandler();
#else
        IInputHandlerBase m_InputHandler = new MouseHandler();
#endif
        public InputManager(Transform container)
        {
            m_Container = container;
        }

        public bool isTouchDown => m_InputHandler.isInputDown;
        public bool isTouchUp => m_InputHandler.isInputUp;
        public Vector2 touchPosition => m_InputHandler.inputPosition;
        public Vector2 touch2BoardPosition => TouchToPosition(m_InputHandler.inputPosition);

        /*
         * 터치 좌표(Screen 좌표)를 보드의 루트인 컨테이너 기준으로 변경된 2차원 좌표를 리턴한다
         * @param vtInput Screen 좌표 즉, 스크린 픽셀 기준 좌표 (좌하(0,0) -> 우상(Screen.Width, Screen.Height))
         * */
        Vector2 TouchToPosition(Vector3 vtInput)
        {
            //1. 스크린 좌표 -> 월드 좌표
            Vector3 vtMousePosW = Camera.main.ScreenToWorldPoint(vtInput);

            //2. 컨테이너 local 좌표계로 변환(컨테이너 위치 이동시에도 컨테이너 기준의 로컬 좌표계이므로 화면 구성이 유연하다)
            Vector3 vtContainerLocal = m_Container.transform.InverseTransformPoint(vtMousePosW);

            return vtContainerLocal;
        }

        public Swipe EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
        {
            return TouchEvaluator.EvalSwipeDir(vtStart, vtEnd);
        }
    }
}
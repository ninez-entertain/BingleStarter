using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Util
{
    public class InputManager
    {
        //public bool isTouchDown { get { return m_InputHandler.isTouchDown; } }

        Transform m_Container;
        IBaseInputHandler m_InputHandler = new MouseHandler();

        public InputManager(Transform container)
        {
            m_Container = container;
        }

        public bool isTouchDown => m_InputHandler.isTouchDown;
        public bool isTouchUp => m_InputHandler.isTouchUp;

        //public bool GetTouchDown()
        //{
        //    return m_InputHandler.GetTouchDown();
        //}

        //public bool GetTouchUp()
        //{
        //    return m_InputHandler.GetTouchUp();
        //}

        public Vector2 touchPosition
        {
            get
            {
                return TouchToPosition(m_InputHandler.touchPosition);
            }
        }

        /*
         * 터치 좌표(Screen 좌표)를 보드의 루트인 컨테이너 기준으로 변경된 2차원 좌표를 리턴한다
         * @param vtInput Screen 좌표 즉, 스크린 픽셀 기준 좌표 (좌하(0,0) -> 우상(Screen.Width, Screen.Height))
         * */
        Vector2 TouchToPosition(Vector3 vtInput)
        {
            //
            Vector3 vtMousePosW = Camera.main.ScreenToWorldPoint(vtInput);

            //컨테이너 local 좌표계로 변환(컨테이너 위치 이동시에도 컨테이너 기준의 로컬 좌표계이므로 화면 구성이 유연하다)
            Vector3 vtContainerLocal = m_Container.transform.InverseTransformPoint(vtMousePosW);

            //Vector2 point = new Vector2(vtContainerLocal.x, vtContainerLocal.y);

            return vtContainerLocal;
        }                       
    }
}
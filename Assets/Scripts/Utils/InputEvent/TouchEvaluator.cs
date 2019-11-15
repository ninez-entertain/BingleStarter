using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Util
{
    /**/
    public enum SwipeDir
    {
        NA      = -1,
        RIGHT   = 0,
        UP      = 1,
        LEFT    = 2,
        DOWN    = 3
    }

    public static class TouchEvaluator
    {
        /*
         * 두 지점을 사용하여 Swipe 방향을 구한다.
         * UP : 45~ 135, LEFT : 135 ~ 225, DOWN : 225 ~ 315, RIGHT : 0 ~ 45, 0 ~ 315
         */
        public static SwipeDir EvalSwipeDir(Vector2 vtStart, Vector2 vtEnd)
        {
            float angle = EvalDragAngle(vtStart, vtEnd);
            if (angle < 0)
                return SwipeDir.NA;

            int swipe = (((int)angle + 45) % 360) / 90;

            switch (swipe)
            {
                case 0: return SwipeDir.RIGHT;
                case 1: return SwipeDir.UP;
                case 2: return SwipeDir.LEFT;
                case 3: return SwipeDir.DOWN;
            }

            return SwipeDir.NA;
        }

        /*
         * 두 포인트 사이의 각도를 구한다.
         * Input(마우스, 터치) 장치 드래그시에 드래그한 각도를 구하는데 활용한다.
         */
        static float EvalDragAngle(Vector2 vtStart, Vector2 vtEnd)
        {
            var dragDirection = vtEnd - vtStart;
            if (dragDirection.magnitude <= 0.2f)
                return -1f;

            //Debug.Log($"eval angle : {vtStart} , {vtEnd}, magnitude = {dragDirection.magnitude}");

            var aimAngle = Mathf.Atan2(dragDirection.y, dragDirection.x);
            if (aimAngle < 0f)
            {
                aimAngle = Mathf.PI * 2 + aimAngle;
            }

            return aimAngle * Mathf.Rad2Deg;
        }
    }
}

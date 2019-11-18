using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Util
{
    public static class Action2D
    {
        /*
         * 지정된 시간동안 지정된 위치로 이동한다.
         * 
         * @param target 애니메이션을 적용할 타겟 GameObject
         * @param to 이동할 목표 위치
         * @param duration 이동 시간
         * @param bSelfRemove 애니메이션 종류 후 타겟 GameObject 삭제 여부 플래그
         */
        public static IEnumerator MoveTo(Transform target, Vector3 to, float duration, bool bSelfRemove = false)
        {
            Vector2 startPos = target.transform.position;

            float elapsed = 0.0f;
            while (elapsed < duration)
            {
                elapsed += Time.smoothDeltaTime;
                target.transform.position = Vector2.Lerp(startPos, to, elapsed / duration);

                yield return null;
            }

            target.transform.position = to;

            if (bSelfRemove)
                Object.Destroy(target.gameObject, 0.1f);

            yield break;
        }
    }
}
//#define SELF_DROP

using System.Collections;
using System.Collections.Generic;
using Ninez.Scriptable;
using Ninez.Util;
using UnityEngine;

namespace Ninez.Board
{
	/*
     * Block GameObject의 애니메이션을 담당한다.
     * - Drop    
     * - Focus, Landing    
     */
	public class BlockActionBehaviour : MonoBehaviour
	{
        public bool isMoving { get; set; }

		Queue<Vector3> m_MovementQueue = new Queue<Vector3>();    //x, y, z = acceleration

		/*
         * 아래쪽으로 주어진 거리만큼 이동한다.
         * fDropDistance : 이동할 스텝 수 즉, 거리 (unit)   
         */
		public void MoveDrop(Vector2 vtDropDistance)
		{
			m_MovementQueue.Enqueue(new Vector3(vtDropDistance.x, vtDropDistance.y, 1));

			if (!isMoving)
			{
                StartCoroutine(DoActionMoveDrop());
            }
		}

        IEnumerator DoActionMoveDrop(float acc = 1.0f)
        {
            isMoving = true;

            while (m_MovementQueue.Count > 0)
            {
                Vector2 vtDestination = m_MovementQueue.Dequeue();

                float duration = Core.Constants.DROP_TIME;// blockConfig.dropSpeed[(int)Mathf.Abs(vtDestination.y) - 1];
                yield return CoStartDropSmooth(vtDestination, duration * acc);
            }

            isMoving = false;
            yield break;
        }

        IEnumerator CoStartDropSmooth(Vector2 vtDropDistance, float duration)
        {
            Vector2 to = new Vector3(transform.position.x + vtDropDistance.x, transform.position.y - vtDropDistance.y);
            yield return Action2D.MoveTo(transform, to, duration);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAgent : MonoBehaviour
{
    [SerializeField] Camera m_TargetCamera;
    [SerializeField] float m_BoardUnit;

    void Start()
    {
        //넓이가 m_BoardUnit을 출력할 수 있도록 카메라 size를 계산한다.
        m_TargetCamera.orthographicSize = m_BoardUnit / m_TargetCamera.aspect;
    }
}

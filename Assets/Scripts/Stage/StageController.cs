using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Stage
{
    public class StageController : MonoBehaviour
    {
        bool m_bInit;
        Stage m_Stage;

        void Start()
        {
            InitStage();
        }

        void InitStage()
        {
            if (m_bInit)
                return;

            m_bInit = true;
        }
    }
}

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

            BuildStage();

            m_Stage.PrintAll();
        }

        void BuildStage()
        {
            //1. Stage를 구성한다.
            m_Stage = StageBuilder.BuildStage(nStage : 0, row : 9, col : 9);

            //2. 생성한 stage 정보를 이용하여 씬을 구성한.
            //stage.ComposeStage(m_BoardConfig, m_Container);
        }
    }
}

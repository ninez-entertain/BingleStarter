using UnityEngine;

namespace Ninez.Stage
{
    public static class StageReader
    {
        /// <summary>
        /// 스테이지 구성을 위해서 구성정보를 로드한다. 
        /// </summary>
        /// <param name="nStage"> 스테이지 번호</param>
        /// <returns></returns>
        public static StageInfo LoadStage(int nStage)
        {
            Debug.Log($"Load Stage : Stage/{GetFileName(nStage)}");

            //1. 리소스 파일에서 텍스트를 읽어온다.
            TextAsset textAsset = Resources.Load<TextAsset>($"Stage/{GetFileName(nStage)}");
            if (textAsset != null)
            {
                //2. JSON 문자열을 객체(StageInfo)로 변환한다.
                StageInfo stageInfo = JsonUtility.FromJson<StageInfo>(textAsset.text);

                //3. 변환된 객체가 유효한지 체크한다(only Debugging)
                Debug.Assert(stageInfo.DoValidation());

                return stageInfo;
            }

            return null;
        }

        /// <summary>
        /// 스테이지 파일 이름을 구한다.
        /// format : stage_0001 -> 4자릿수 네이밍 적용, 리소스 파일에서 로딩하는 경우 확장자 생략
        /// </summary>
        /// <param name="nStage"></param>
        /// <returns></returns>
        static string GetFileName(int nStage)
        {
            return string.Format("stage_{0:D4}", nStage);
        }
    }
}

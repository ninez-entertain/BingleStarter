using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Scriptable
{
    [CreateAssetMenu(menuName = "Bingle/Block Config", fileName = "BlockConfig.asset")]
    public class BlockConfig : ScriptableObject
    {
        public Sprite[] basicBlockSprites;

        //public SpriteKV[] blockSprites;

        //private Dictionary<string, Sprite> m_SpriteMap;

        //public void OnEnable()
        //{
        //    Init();
        //}

        //public void Init()
        //{
        //    if (m_SpriteMap == null)
        //    {
        //        m_SpriteMap = new Dictionary<string, Sprite>();

        //        //빠른 조회를 위해 스프라이트를 Dictionary에 추가한다. 
        //        foreach (SpriteKV cb in blockBgSprites)
        //            m_SpriteMap.Add(cb.name, cb.sprite);
        //    }

        //    Debug.Log("BlockConfig Init");
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public Sprite GetSprite(string name)
        //{
        //    if (m_SpriteMap.ContainsKey(name))
        //        return m_SpriteMap[name];

        //    Debug.Assert(false, $"Block sprite Not assigned : {name}");

        //    return null;
        //}
    }
}
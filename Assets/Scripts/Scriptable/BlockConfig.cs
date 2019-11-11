using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Scriptable
{
    [CreateAssetMenu(menuName = "Bingle/Block Config", fileName = "BlockConfig.asset")]
    public class BlockConfig : ScriptableObject
    {
        public Sprite[] basicBlockSprites;
    }
}
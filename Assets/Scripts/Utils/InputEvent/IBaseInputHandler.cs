using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Util
{
    public interface IBaseInputHandler
    {
        bool isTouchDown { get; }
        bool isTouchUp{ get; }
        Vector2 touchPosition { get; }

        //bool GetTouchDown();
        //bool GetTouchUp();
    }
}

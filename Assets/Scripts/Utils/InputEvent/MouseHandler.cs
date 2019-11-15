using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Util
{
    public class MouseHandler : IBaseInputHandler
    {
        bool IBaseInputHandler.isTouchDown => Input.GetButtonDown("Fire1");
        bool IBaseInputHandler.isTouchUp => Input.GetButtonUp("Fire1");

        Vector2 IBaseInputHandler.touchPosition => Input.mousePosition;
    }
}

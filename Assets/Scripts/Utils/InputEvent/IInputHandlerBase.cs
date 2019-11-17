using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ninez.Util
{
    public interface IInputHandlerBase
    {
        bool isInputDown { get; }
        bool isInputUp{ get; }
        Vector2 inputPosition { get; } //Screen(픽셀) 좌표
    }
}

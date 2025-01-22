using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsHelper
{
    public static float FindDistance(int AX, int AY, int BX, int BY)
    {
        int XDiff = BX - AX;
        int YDiff = BY - AY;
        return Mathf.Sqrt((XDiff * XDiff) + (YDiff * YDiff));
    }
}

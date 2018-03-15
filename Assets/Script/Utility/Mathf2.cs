using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Mathf2
{
    public static float ReverseSmooth(float x)
    {
        return x + (x - (x * x * (3.0f - 2.0f * x)));
    }


}

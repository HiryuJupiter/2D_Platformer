﻿using UnityEngine;
using System.Collections;
using UnityEngine.Assertions.Must;

public static class MathsUtil
{
    //Similar to Mathf.Sign but allows zero
    public static int SignAllowingZero (float value)
    {
        if (value > 0.1f)
            return 1;
        else if (value < -0.1f)
            return -1;
        else
            return 0;
    }
}
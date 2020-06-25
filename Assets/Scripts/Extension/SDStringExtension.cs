using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SDStringExtension
{
    public static bool IsNotEmpty(this string str)
    {
        return !string.IsNullOrEmpty(str);
    }
}

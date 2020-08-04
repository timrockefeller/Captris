using UnityEngine;
public static class ExtendFunctions
{
    public static float Sigmoid(this float num)
    {
        return 1.0f / (1.0f + Mathf.Exp(-num));
    }
}
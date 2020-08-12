using UnityEngine;
// Random Util
// seed can be set only once
public static class RD
{
    private static System.Random _RD = null;
    private static int seed;
    public static double NextDouble()
    {
        if (_RD != null)
            return _RD.NextDouble();
        return new System.Random().NextDouble();
    }

    public static float NextFloat()
    {
        return (float)_RD.NextDouble();
    }
    /// <summary>
    /// return a integer from 0 to max-1
    /// </summary>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int NextInt(int max)
    {
        return (int)(_RD.NextDouble() * max);
    }
    public static void SetSeed(int _seed)
    {
        if (_RD != null) return;
        seed = _seed;
        _RD = new System.Random(_seed);
    }
    public static void SetSeedS(int _seed)
    {
        seed = _seed;
        _RD = new System.Random(_seed);
    }
    public static Vector2Int NextPosition(int x, int y)
    {
        return new Vector2Int((int)(NextDouble() * x), (int)(NextDouble() * y));
    }
    public static Vector2 NextPositionf(float x, float y)
    {
        return new Vector2((float)(NextDouble() * x), (float)(NextDouble() * y));
    }
    public static Vector3 NextPositionf(float x, float y, float z)
    {
        return new Vector3((float)(NextDouble() * x), (float)(NextDouble() * y), (float)(NextDouble() * z));
    }
}

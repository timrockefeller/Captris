// Random Util
// seed can be set only once
public static class RD
{
    private static System.Random _RD = null;
    private static int seed;
    public static double NextDouble()
    {
        return _RD.NextDouble();
    }
    public static float NextFloat()
    {
        return (float)_RD.NextDouble();
    }
    public static void SetSeed(int _seed)
    {
        if (_RD != null) return;
        seed = _seed;
        _RD = new System.Random(_seed);
    }
}
// Random Util
// seed can be set only once
public static class RD {
    private static System.Random _RD;
    private static int seed;
    public static double NextDouble(){
        return _RD.NextDouble();
    }
    public static void SetSeed(int _seed){
        seed = _seed;
        _RD = new System.Random(_seed);
    }
}
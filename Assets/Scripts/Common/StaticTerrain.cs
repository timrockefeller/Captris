public static class StaticTerrain
{
    static UnitType R = UnitType.Rock;
    static UnitType O = UnitType.Empty;
    static UnitType T = UnitType.Tower;
    static UnitType V = UnitType.Void;
    static UnitType M = UnitType.Mine;
    public static UnitType[][,] towers = {
        new UnitType [,]{
            {R,R,R,R,R},
            {R,V,V,V,R},
            {R,V,T,V,R},
            {R,V,O,V,R},
            {R,R,O,R,R},
        },
        new UnitType [,]{
            {O,R,O,R,O},
            {R,R,O,R,R},
            {O,O,T,O,O},
            {R,R,O,R,R},
            {O,R,O,R,O},
        },
        // new UnitType [,]{
        //     {G,G,G,G,G},
        //     {G,R,O,R,G},
        //     {G,O,T,O,G},
        //     {G,R,O,R,G},
        //     {G,G,G,G,G},
        // },
    };
    public static UnitType[][,] modules = {
        new UnitType [,]{
            {O,R,O,O,O},
            {R,M,O,O,O},
            {R,O,O,R,O},
            {O,O,O,O,O},
            {O,O,O,O,O},
        }, new UnitType [,]{
            {O,O,O,O,O},
            {O,O,R,R,O},
            {O,O,M,R,O},
            {O,O,O,O,O},
            {O,O,O,O,O},
        }, new UnitType [,]{
            {O,R,O,O,O},
            {R,M,O,O,O},
            {R,O,O,R,O},
            {O,O,O,O,O},
            {O,O,O,O,O},
        }, new UnitType [,]{
            {O,R,O,O,O},
            {R,M,M,O,O},
            {O,O,O,R,O},
            {O,O,O,O,O},
            {O,O,O,O,O},
        }, new UnitType [,]{
            {O,O,O,O,R},
            {O,O,O,M,O},
            {O,O,O,M,R},
            {O,O,O,R,O},
            {O,O,O,O,O},}
        // }, new UnitType [,]{
        //     {O,O,O,O,O},
        //     {O,O,R,O,O},
        //     {O,R,R,R,O},
        //     {O,O,O,O,O},
        //     {O,O,O,O,O},
        // }, new UnitType [,]{
        //     {O,O,O,O,O},
        //     {O,O,R,O,O},
        //     {O,O,R,R,O},
        //     {O,O,O,R,O},
        //     {O,O,O,O,O},
        // }, new UnitType [,]{
        //     {O,O,O,O,O},
        //     {O,O,R,O,O},
        //     {O,R,R,O,O},
        //     {O,R,O,O,O},
        //     {O,O,O,O,O},
        // }, new UnitType [,]{
        //     {O,O,O,O,O},
        //     {O,R,R,O,O},
        //     {O,R,R,O,O},
        //     {O,O,O,O,O},
        //     {O,O,O,O,O},
        // }, new UnitType [,]{
        //     {O,O,O,O,O},
        //     {O,R,O,O,O},
        //     {O,R,R,R,O},
        //     {O,O,O,O,O},
        //     {O,O,O,O,O},
        // }, new UnitType [,]{
        //     {O,O,O,O,O},
        //     {O,O,O,O,O},
        //     {R,R,R,R,O},
        //     {O,O,O,O,O},
        //     {O,O,O,O,O},
        // }, new UnitType [,]{
        //     {O,O,O,O,O},
        //     {O,O,O,R,O},
        //     {O,R,R,R,O},
        //     {O,O,O,O,O},
        //     {O,O,O,O,O},
        // }
    };
    public static UnitType[,] NextTower()
    {
        int rn = RD.NextInt(towers.GetLength(0));
        return RotateMatrix(towers[rn], RD.NextInt(4));
    }

    public static UnitType[,] NextModule()
    {
        int rn = RD.NextInt(modules.GetLength(0));
        return RotateMatrix(modules[rn], RD.NextInt(4));
    }

    /// <summary>
    /// 旋转矩阵
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="rotation">clockwised (rotation * 90)</param>
    /// <returns></returns>
    private static UnitType[,] RotateMatrix(UnitType[,] mat, int rotation)
    {
        if (rotation == 0) return mat;
        bool sameSize = rotation % 2 == 0;
        UnitType[,] rst = new UnitType[mat.GetLength(sameSize ? 0 : 1), mat.GetLength(sameSize ? 1 : 0)];
        for (int x = 0; x < mat.GetLength(0); x++)
        {
            for (int y = 0; y < mat.GetLength(1); y++)
            {
                int curx = sameSize ? (rotation == 0 ? x : mat.GetLength(0) - 1 - x) : (rotation == 1 ? y : mat.GetLength(1) - 1 - y);
                int cury = sameSize ? (rotation == 0 ? y : mat.GetLength(1) - 1 - y) : (rotation == 1 ? x : mat.GetLength(0) - 1 - x);
                rst[curx, cury] = mat[x, y];
            }
        }
        return rst;
    }
}
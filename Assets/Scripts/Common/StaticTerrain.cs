using System.Linq.Expressions;
using System;

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
        },
        new UnitType [,]{
            {O,O,O,O,O},
            {O,O,R,R,O},
            {O,O,M,R,O},
            {O,O,O,O,O},
            {O,O,O,O,O},
        },
        new UnitType [,]{
            {O,R,O,O,O},
            {R,M,O,O,O},
            {R,O,O,R,O},
            {O,O,O,O,O},
            {O,O,O,O,O},
        },
        new UnitType [,]{
            {O,R,O,O,O},
            {R,M,M,O,O},
            {O,O,O,R,O},
            {O,O,O,O,O},
            {O,O,O,O,O},
        },
        new UnitType [,]{
            {O,O,O,O,R},
            {O,O,O,M,O},
            {O,O,O,M,R},
            {O,O,O,R,O},
            {O,O,O,O,O},
        }
    };
    public static UnitType[,] NextTower()
    {
        double rn = RD.NextDouble() * towers.GetLength(0);
        return towers[(int)rn];
    }

    public static UnitType[,] NextModule()
    {
        double rn = RD.NextDouble() * modules.GetLength(0);
        return modules[(int)rn];
    }
}
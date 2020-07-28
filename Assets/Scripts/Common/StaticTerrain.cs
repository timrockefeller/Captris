using System;

public static class StaticTerrain
{
    public static int[][,] modules = {
        new int [,]{
   
        }
    };

    public static Int32[,] NextModule()
    {



        double rn = RD.NextDouble() * modules.GetLength(0);



        return modules[(int)rn];
    }
}
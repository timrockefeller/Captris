using UnityEngine;
using System.Collections;

public class RandomMountain : MonoBehaviour
{

    void Start()
    {
        RD.SetSeed((int)(Random.value*10000));
        Vector3[] newVertices = new Vector3[64];
        Vector2[] newUV = new Vector2[64];

        int[] newTriangles = new int[7 * 7 * 2 * 3];
        float _seedX = (float)(RD.NextDouble() * 100.0);
        float _seedZ = (float)(RD.NextDouble() * 100.0);
        float _relief = 1;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                float distance = (new Vector2(i, j) - new Vector2(3.5f, 3.5f)).magnitude;
                float attenuation = 1-Mathf.Clamp01(distance / 3.5f);

                float xSample = (i + _seedX) / _relief;
                float zSample = (j + _seedZ) / _relief;
                float noise = Mathf.PerlinNoise(xSample, zSample);
                newVertices[i * 8 + j] = new Vector3(0.25f +(i / 14.0f), noise * attenuation, 0.25f +(j / 14.0f));
                newUV[i * 8 + j] = new Vector2(i / 7.0f, j / 7.0f);
                if (i < 7 && j < 7)
                {
                    newTriangles[(i * 7 + j) * 6 + 0] = i * 8 + j;
                    newTriangles[(i * 7 + j) * 6 + 1] = i * 8 + j + 1;
                    newTriangles[(i * 7 + j) * 6 + 2] = i * 8 + j + 8;
                    newTriangles[(i * 7 + j) * 6 + 3] = i * 8 + j + 1;
                    newTriangles[(i * 7 + j) * 6 + 4] = i * 8 + j + 9;
                    newTriangles[(i * 7 + j) * 6 + 5] = i * 8 + j + 8;
                }
            }
        }


        // { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0) };
        // Vector2[] newUV = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) };
        // int[] newTriangles = { 0, 2, 1, 0, 3, 2 };

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
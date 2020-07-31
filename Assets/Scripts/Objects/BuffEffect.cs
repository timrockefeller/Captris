using System;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MeshFilter))]
public class BuffEffect : MonoBehaviour
{
    void Start()
    {

    }
    List<Vector3Int> _lines;
    public void AttachMesh(List<Vector3Int> lines, float height = 1F)
    {


#if UNITY_EDITOR
        _lines = lines;
#endif

        Vector3[] newVertices = new Vector3[lines.Count * 2];
        Vector2[] newUV = new Vector2[lines.Count * 2];
        int[] newTriangles = new int[lines.Count * 3];
        for (int i = 0; i < lines.Count / 2; i++)
        {
            newVertices[4 * i] = GameUtils.PositionToPoint(lines[2 * i]);
            newVertices[4 * i + 1] = GameUtils.PositionToPoint(lines[2 * i + 1]);
            newVertices[4 * i + 2] = GameUtils.PositionToPoint(lines[2 * i] + Vector3.up * height);
            newVertices[4 * i + 3] = GameUtils.PositionToPoint(lines[2 * i + 1] + Vector3.up * height);

            newUV[4 * i] = new Vector2(0, 1);
            newUV[4 * i + 1] = new Vector2(1, 1);
            newUV[4 * i + 2] = new Vector2(0, 0);
            newUV[4 * i + 3] = new Vector2(1, 0);

            newTriangles[6 * i] = 4 * i;
            newTriangles[6 * i + 1] = 4 * i + 1;
            newTriangles[6 * i + 2] = 4 * i + 2;

            newTriangles[6 * i + 3] = 4 * i + 3;
            newTriangles[6 * i + 4] = 4 * i + 2;
            newTriangles[6 * i + 5] = 4 * i + 1;
        }

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        mesh.name = "Generated Buff Border";
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public void LoadMeshByBlocks(List<Vector3Int> l, Color color)
    {

        AttachMesh(BuffEffectManager.GetLineByVectors(l));
        this.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    private void Update()
    {
#if UNITY_EDITOR
        for (int i = 0; i < _lines.Count / 2; i++)
        {
            Debug.DrawLine(GameUtils.PositionToPoint(_lines[2 * i]), GameUtils.PositionToPoint(_lines[2 * i + 1]), Color.red);
        }

#endif
    }
}
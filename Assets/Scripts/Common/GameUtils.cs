using UnityEngine;


static class GameUtils
{
    public static Vector3 PositionToTranform(Vector3Int pos)
    {
        return new Vector3(pos.x + 0.5f, pos.y / 2.0f - 0.5f, pos.z + 0.5f);
    }
    public static Vector3 PositionToPoint(Vector3 pos)
    {
        return new Vector3(pos.x, pos.y / 2.0f, pos.z);
    }
}

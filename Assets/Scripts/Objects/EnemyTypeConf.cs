using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EnemyType
{
    Enemy_Lazer = 1,
    Enemy_Giant = 2,
    Enemy_Tower = 3
}
public class EnemyTypeConf : MonoBehaviour
{
    public EnemyType type;
}
